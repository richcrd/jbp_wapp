using jbp_wapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using jbp_wapp.Data;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace jbp_wapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Verifica si el usuario esta autenticado con la sesion
            var usuarioID = HttpContext.Session.GetInt32("UsuarioID");
            if (usuarioID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Busar el usuario autenticado en la BD
            var usuario = _context.Usuarios.Find(usuarioID);

            if (usuario == null)
            {
                // Si el usuario no esta en la BD, redirigir
                return RedirectToAction("Login", "Account");
            }
            // Pasar el usuario a la vista
            return View(usuario);
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