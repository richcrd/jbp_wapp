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
                  var usuario = await _context.Usuarios
                    .Include(u => u.Departamento)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                  if (usuario == null)
                  {
                      return NotFound("Usuario no encontrado.");
                  }

                  return View("UsuarioIndex", usuario);
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
        public async Task<IActionResult> EditPostulante()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }
            var userId = int.Parse(userIdClaim);

            var postulante = await _context.PerfilPostulante
                .Include(p => p.Profesion)
                .Include(p => p.Experiencia)
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Departamento)
                .FirstOrDefaultAsync(p => p.IdUsuario == userId);

            if (postulante == null)
            {
                return NotFound("Perfil de postulante no encontrado.");
            }

            ViewBag.Profesiones = await _context.Profesiones
            .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre,
                    Selected = p.Id == postulante.IdProfesion
                }).ToListAsync();

            ViewBag.Experiencias = await _context.Experiencias
            .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Descripcion,
                    Selected = e.Id == postulante.IdExperiencia
                }).ToListAsync();

            ViewBag.Departamentos = await _context.Departamentos
            .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Nombre,
                    Selected = d.Id == postulante.Usuario.IdDepartamento
                }).ToListAsync();

            return View(postulante);
        }

        // Acción para guardar los cambios en el perfil del postulante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPostulante(PerfilPostulante model, IFormFile cvfile)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }
        
            var postulante = await _context.PerfilPostulante
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == model.Id);
        
            if (postulante == null)
            {
                return NotFound("Perfil de postulante no encontrado.");
            }
        
            // Actualizar solo campos proporcionados
            if (cvfile != null && cvfile.Length > 0)
            {
                using var ms = new MemoryStream();
                await cvfile.CopyToAsync(ms);
                postulante.CV = ms.ToArray();
            }

            if (model.IdProfesion > 0)
                postulante.IdProfesion = model.IdProfesion;

            if (model.IdExperiencia > 0)
                postulante.IdExperiencia = model.IdExperiencia;

            // Actualizar campos del usuario relacionado
            if (!string.IsNullOrEmpty(model.Usuario.Nombre))
                postulante.Usuario.Nombre = model.Usuario.Nombre;

            if (!string.IsNullOrEmpty(model.Usuario.Apellido))
                postulante.Usuario.Apellido = model.Usuario.Apellido;

            if (!string.IsNullOrEmpty(model.Usuario.Correo))
                postulante.Usuario.Correo = model.Usuario.Correo;

            _context.Entry(postulante).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar cambios: {ex.Message}");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditUsuario()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }
            var userId = int.Parse(userIdClaim);

            var usuario = await _context.Usuarios
                .Include(u => u.Departamento)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            ViewBag.Departamentos = await _context.Departamentos.Select(d => new SelectListItem{
                Value = d.Id.ToString(),
                Text = d.Nombre,
                Selected = d.Id == usuario.IdDepartamento
            }).ToListAsync();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUsuario(Usuario model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
                return View(model);
            }

            var usuario = await _context.Usuarios.FindAsync(model.Id);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Actualizar campos requeridos
            if (!string.IsNullOrEmpty(model.NombreUsuario))
                usuario.NombreUsuario = model.NombreUsuario;

            if (!string.IsNullOrEmpty(model.Contrasena))
                usuario.Contrasena = model.Contrasena;

            // Actualizar campos opcionales
            if (!string.IsNullOrEmpty(model.Nombre))
                usuario.Nombre = model.Nombre;
            if (!string.IsNullOrEmpty(model.Apellido))
                usuario.Apellido = model.Apellido;
            if (!string.IsNullOrEmpty(model.Correo))
                usuario.Correo = model.Correo;
            if (model.IdDepartamento > 0)
                usuario.IdDepartamento = model.IdDepartamento;

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DownloadCV(int id)
        {
            var perfil = _context.PerfilPostulante.FirstOrDefaultAsync(p => p.Id == id);

            if (perfil == null || perfil.CV == null)
            {
                return NotFound("El cv no esta disponible");
            }

            var fileName = $"CV_{perfil.Usuario?.Nombre ?? "Postulante"}_{perfil.Usuario?.Apellido ?? "Desconocido"}.pdf";

            // Retornar el archivo como un contenido descargable
            return File(perfil.CV, "application/pdf", fileName);
        }
    }
}