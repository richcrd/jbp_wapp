using jbp_wapp.Data;
using jbp_wapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace jbp_wapp.Controllers
{

    public class VacanteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VacanteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vacante/Index - Muestra todas las vacantes
        [HttpGet]
        [Authorize(Roles = "3")]
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
        [Authorize(Roles = "1,2")]
        [HttpGet] // Autoriza a cualquier usuario pero que este autenticado
        public async Task<IActionResult> All(string keyword, int? idExp, int? idProf)
        {
            // Inicializamos la consulta como IQueryable
            IQueryable<Vacante> query = _context.Vacantes
                .Include(v => v.Usuario)
                .Include(v => v.Profesion)
                .Include(v => v.Experiencia);

            // Aplicar filtros si están presentes
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(v => v.Titulo.Contains(keyword) || v.Descripcion.Contains(keyword));
            }

            if (idExp.HasValue)
            {
                query = query.Where(v => v.IdExperiencia == idExp.Value);
            }

            if (idProf.HasValue)
            {
                query = query.Where(v => v.IdProfesion == idProf.Value);
            }

            // Obtener la lista de resultados
            var listaVacantes = await query.ToListAsync();

            // Enviar listas de selección para filtros
            ViewData["Keyword"] = keyword;
            ViewBag.Experiencias = new SelectList(await _context.Experiencias.ToListAsync(), "Id", "Descripcion", idExp);
            ViewBag.Profesiones = new SelectList(await _context.Profesiones.ToListAsync(), "Id", "Nombre", idProf);

            return View(listaVacantes);
        }
        
        // GET: Vacante/Create - Muestra el formulario de creación de vacantes
        [HttpGet]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Create()
        {
            await CargarDatos();
            return View();
        }

        // POST: Vacante/Create - Permite a los reclutadores crear una vacante
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Create(Vacante vacante)
        {
            await CargarDatos();

            if(vacante.Aplicaciones == null)
            {
                vacante.Aplicaciones = new List<Aplicacion>();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await CargarDatos();
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        vacante.IdUsuario = int.Parse(userIdClaim.Value);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Nose pudo obtener el Id del usuario");
                        return View(vacante);
                    }

                    var vacantesExistente_Reclutador = await _context.Vacantes
                        .Where(v => v.IdUsuario == vacante.IdUsuario)
                        .CountAsync();

                    if (vacantesExistente_Reclutador >= 2)
                    {

                        ViewBag.ErrorMessage = "Haz alcanzado el limite de vacantes. No puedes crear mas de 2 vacantes";
                        return View(vacante);
                    }

                    vacante.FechaCreacion = DateTime.Now;
                    _context.Add(vacante);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Vacante");
                }
                catch (Exception ex) 
                {
                    ViewBag.ErrorMessage = "Error en el modelo";
                    Console.WriteLine("Error", ex);
                    return View(vacante);
                }
                
            }
            return View(vacante);
        }

        public async Task CargarDatos()
        {
            ViewBag.Experiencias = await _context.Experiencias.ToListAsync();
            ViewBag.Profesiones = await _context.Profesiones.ToListAsync();
        }

        // Acción para eliminar una vacante
        [HttpGet]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Delete(int id)
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

        // Acción para confirmar la eliminación
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "3")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacante = await _context.Vacantes
                .Include(v => v.Aplicaciones)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vacante == null)
            {
                return NotFound();
            }
            // Opcional: Eliminar las aplicaciones asociadas a la vacante antes de eliminar la vacante
            if (vacante.Aplicaciones != null && vacante.Aplicaciones.Any())
            {
                _context.Aplicaciones.RemoveRange(vacante.Aplicaciones);
            }
            _context.Vacantes.Remove(vacante);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Vacante");
        }

        private bool VacanteExists(int id)
        {
            return _context.Vacantes.Any(e => e.Id == id);
        }

        [Authorize(Roles = "1,2,3")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vacante = await _context.Vacantes
            .Include(v => v.Usuario)
            .Include(v => v.Profesion)
            .Include(v => v.Experiencia)
            .FirstOrDefaultAsync(v => v.Id == id);
            
            if (vacante == null)
            {
                return NotFound("No se encontro la vacante con este id");
            }

            return PartialView("_VacanteDetails", vacante);
        }
    }
}
