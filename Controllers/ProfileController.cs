using jbp_wapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using jbp_wapp.Data;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace jbp_wapp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }
            var userId = int.Parse(userIdClaim);

            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole == "2")
            {
                  // Cargar perfil de postulante
                  var postulante = await _context.PerfilPostulante
                      .Include(p => p.Profesion)
                      .Include(p => p.Experiencia)
                      .Include(p => p.Usuario)
                      .ThenInclude(u => u.Departamento)
                      .FirstOrDefaultAsync(p => p.IdUsuario == userId);

              if (postulante == null)
              {
                return RedirectToAction(nameof(Create));
              }

              return View(postulante);
              
            }
            else
            {
              // Para Admin y Reclutador, cargar solo información básica del usuario
                  var usuario = await _context.Usuarios.FindAsync(userId);
                  if (usuario == null)
                  {
                      return NotFound("Usuario no encontrado.");
                  }

                  return View("Index", usuario);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }
            var userId = int.Parse(userIdClaim);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userRole != "2")
            {
                return Unauthorized("Solo los postulantes pueden crear un perfil.");
            }

            ViewBag.Profesiones = await _context.Profesiones.ToListAsync();
            ViewBag.Experiencias = await _context.Experiencias.ToListAsync();
            ViewBag.Departamentos = await _context.Departamentos.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PerfilPostulante pfp, IFormFile cvfile)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }
            var userId = int.Parse(userIdClaim);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            pfp.IdUsuario = userId;

            if (userRole != "2")
            {
                return Forbid("Solo los postulantes pueden crear un perfil.");
            }
           
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                ViewBag.Profesiones = await _context.Profesiones.ToListAsync();
                ViewBag.Experiencias = await _context.Experiencias.ToListAsync();
                ViewBag.Departamentos = await _context.Departamentos.ToListAsync();
                return View(pfp);
            }

            if (cvfile != null)
            {
                Console.WriteLine($"Archivo recibido: {cvfile.FileName}, tamaño: {cvfile.Length}");
                using var ms = new MemoryStream();
                await cvfile.CopyToAsync(ms);
                pfp.CV = ms.ToArray();
            }
            else
            {
                Console.WriteLine("cvFile", "Debes subir un archivo de CV.");
                ViewBag.Profesiones = await _context.Profesiones.ToListAsync();
                ViewBag.Experiencias = await _context.Experiencias.ToListAsync();
                ViewBag.Departamentos = await _context.Departamentos.ToListAsync();
                return View(pfp);
            }

            _context.PerfilPostulante.Add(pfp);
            Console.WriteLine("Guardando perfil...");
            await _context.SaveChangesAsync();
            Console.WriteLine("Perfil guardado con ID: " + pfp.Id);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userRole == "Postulante")
            {
                var postulante = await _context.PerfilPostulante
                    .Include(p => p.Profesion)
                    .Include(p => p.Experiencia)
                    .Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.IdUsuario == userId);

                if (postulante == null)
                {
                    return RedirectToAction(nameof(Create));
                }

                ViewBag.Profesiones = await _context.Profesiones.ToListAsync();
                ViewBag.Experiencias = await _context.Experiencias.ToListAsync();
                ViewBag.Departamentos = await _context.Departamentos.ToListAsync();

                return View("PostulanteEdit", postulante);
            }
            else
            {
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                ViewBag.Departamentos = await _context.Departamentos.ToListAsync();
                return View("Edit", usuario);
            }
        }

        // Acción para guardar los cambios en el perfil del postulante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PerfilPostulante pfp, Usuario usuario, IFormFile cvFile)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!ModelState.IsValid)
            {
                ViewBag.Profesiones = await _context.Profesiones.ToListAsync();
                ViewBag.Experiencias = await _context.Experiencias.ToListAsync();
                ViewBag.Departamentos = await _context.Departamentos.ToListAsync();
                return View(userRole == "Postulante" ? "PostulanteEdit" : "UsuarioEdit");
            }

            if (userRole == "Postulante")
            {
                var perfilExistente = await _context.PerfilPostulante
                    .Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.IdUsuario == userId);

                if (perfilExistente == null)
                {
                    return RedirectToAction(nameof(Create));
                }

                perfilExistente.IdProfesion = pfp.IdProfesion;
                perfilExistente.IdExperiencia = pfp.IdExperiencia;
                perfilExistente.Usuario.Nombre = usuario.Nombre;
                perfilExistente.Usuario.Apellido = usuario.Apellido;
                perfilExistente.Usuario.Correo = usuario.Correo;
                perfilExistente.Usuario.IdDepartamento = usuario.IdDepartamento;

                if (cvFile != null && cvFile.Length > 0)
                {
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        await cvFile.CopyToAsync(memoryStream);
                        perfilExistente.CV = memoryStream.ToArray();
                    }
                }

                _context.Update(perfilExistente);
            }
            else
            {
                var usuarioExistente = await _context.Usuarios.FindAsync(userId);
                if (usuarioExistente == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Apellido = usuario.Apellido;
                usuarioExistente.Correo = usuario.Correo;
                usuarioExistente.IdDepartamento = usuario.IdDepartamento;

                _context.Update(usuarioExistente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}