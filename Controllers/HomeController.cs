using jbp_wapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using jbp_wapp.Data;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}