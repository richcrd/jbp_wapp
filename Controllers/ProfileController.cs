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
          var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
          var postulante = await _context.PerfilPostulante
            .Include(p => p.Profesion)
            .Include(p => p.Experiencia)
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.IdUsuario == userId);

          if(postulante == null)
          {
            return NotFound("Perfil de postulante no encontrado");
          }

          return View(postulante);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
          var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
          var postulante = await _context.PerfilPostulante.FindAsync(userId);

          var pp = await _context.PerfilPostulante
                .Include(p => p.Profesion)
                .Include(p => p.Experiencia)
                .FirstOrDefaultAsync(p => p.IdUsuario == userId);

            if (pp == null)
            {
                return NotFound("Perfil de postulante no encontrado.");
            }

            ViewBag.Profesiones = await _context.Profesiones.ToListAsync();
            ViewBag.Experiencias = await _context.Experiencias.ToListAsync();

            return View(pp);
        }

        // Acción para guardar los cambios en el perfil del postulante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PerfilPostulante pfp, IFormFile cvFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Profesiones = await _context.Profesiones.ToListAsync();
                ViewBag.Experiencias = await _context.Experiencias.ToListAsync();
                return View(pfp);
            }

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var perfilExistente = await _context.PerfilPostulante.FirstOrDefaultAsync(p => p.IdUsuario == userId);

            if (perfilExistente == null)
            {
                return NotFound("Perfil de postulante no encontrado.");
            }

            perfilExistente.IdProfesion = pfp.IdProfesion;
            perfilExistente.IdExperiencia = pfp.IdExperiencia;

            // Si se subió un nuevo CV, reemplazar el archivo existente
            if (cvFile != null && cvFile.Length > 0)
            {
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    await cvFile.CopyToAsync(memoryStream);
                    perfilExistente.CV = memoryStream.ToArray();
                }
            }

            _context.Update(perfilExistente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        } 
    }
}