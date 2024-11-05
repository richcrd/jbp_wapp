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
    [Authorize(Roles = "3")]
    public class VacanteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VacanteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vacante/Index - Muestra todas las vacantes
        [HttpGet]
        public async Task<IActionResult> Index()
        {
          await CargarDatos();
          var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
          if (userIdClaim == null) 
          {
            // Manejar aqui cuando no se pudo obtener
            return RedirectToAction("Login", "Account");
          }
          var userId = int.Parse(userIdClaim.Value);

            var vacantes = await _context.Vacantes
                .Where(v => v.IdUsuario == userId)
                .Include(v => v.Usuario)
                .Include(v => v.Profesion)
                .Include(v => v.Experiencia)
                .ToListAsync();

            return View(vacantes);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All()
        {
            var vacantes = await _context.Vacantes
                .Include(v => v.Usuario)
                .Include(v => v.Profesion)
                .Include(v => v.Experiencia)
                .ToListAsync();

            return View(vacantes);
        }
        
        // GET: Vacante/Create - Muestra el formulario de creación de vacantes
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarDatos();
            return View();
        }

        // POST: Vacante/Create - Permite a los reclutadores crear una vacante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vacante vacante)
        {
            await CargarDatos();

            if (ModelState.IsValid)
            {
                await CargarDatos();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if(userIdClaim != null)
                {
                    vacante.IdUsuario = int.Parse(userIdClaim.Value);
                }
                else 
                {
                    ModelState.AddModelError("", "Nose pudo obtener el Id del usuario");
                    return View(vacante);
                }
                vacante.FechaCreacion = DateTime.Now;
                _context.Add(vacante);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Vacante");
            }
            return View(vacante);
        }

        public async Task CargarDatos()
        {
            ViewBag.Experiencias = await _context.Experiencias.ToListAsync();
            ViewBag.Profesiones = await _context.Profesiones.ToListAsync();
        }

        // GET: Vacante/Details/{id} - Muestra los detalles de una vacante
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vacante = await _context.Vacantes
                .Include(v => v.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vacante == null)
            {
                return NotFound();
            }

            return View(vacante);
        }

         // Acción para mostrar el formulario de edición
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vacante = await _context.Vacantes.FindAsync(id);
            if (vacante == null)
            {
                return NotFound();
            }
            return View(vacante);
        }

        // Acción para procesar el formulario de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vacante vacante)
        {
            if (id != vacante.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacante);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacanteExists(vacante.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vacante);
        }

        // Acción para eliminar una vacante
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vacante = await _context.Vacantes.FindAsync(id);
            if (vacante == null)
            {
                return NotFound();
            }
            return View(vacante);
        }

        // Acción para confirmar la eliminación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacante = await _context.Vacantes.FindAsync(id);
            _context.Vacantes.Remove(vacante);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Vacante");
        }

        private bool VacanteExists(int id)
        {
            return _context.Vacantes.Any(e => e.Id == id);
        }
    }
}
