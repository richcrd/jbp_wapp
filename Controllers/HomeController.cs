using jbp_wapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using jbp_wapp.Data;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
=======
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
>>>>>>> feature/ui

namespace jbp_wapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción para la pantalla principal (Index)
<<<<<<< HEAD
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            Console.WriteLine($"Usuario ID en sesión: {userId}");

            // Verifica si el usuario está autenticado
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirige a la pantalla de inicio de sesión si no está autenticado
            }
            // Obtener el nombre del usuario de la sesión
            var usuarioNombre = HttpContext.Session.GetString("UsuarioNombre");
            ViewData["UsuarioNombre"] = usuarioNombre;
            return View();
        }
=======
        public async Task<IActionResult> Index()
        {
            // Verifica si el usuario está autenticado
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Redirige a la pantalla de inicio de sesión si no está autenticado
            }

            // Extraer los datos del usuario autenticado desde los claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            ViewData["UserId"] = userId;
            ViewData["UserName"] = userName;
            ViewData["UserRole"] = userRole;

            ViewBag.VacantesRecomendadas = await _context.Vacantes
                .Where(v => v.IdExperiencia == 1)
                .ToListAsync();
>>>>>>> feature/ui

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}