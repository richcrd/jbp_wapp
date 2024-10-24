using jbp_wapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using jbp_wapp.Data;

namespace jbp_wapp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string correo, string contrasena)
        {
            try
            {
                var usuario = _context.Usuarios.SingleOrDefault(u => u.CorreoUsuario == correo && u.ContrasenaUsuario == contrasena);

                if (usuario != null)
                {
                    // Iniciar sesion
                    Console.WriteLine($"Usuario encontrado: {usuario.NombreUsuario} {usuario.ContrasenaUsuario}");
                    HttpContext.Session.SetInt32("UsuarioID", usuario.UsuarioID);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Console.WriteLine("Credenciales incorrectas");
                }
                ModelState.AddModelError("", "Correo o contrasena incorrectos");
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Se produjo un error en el inicio de sesión.");
                return View();
            }
        }

        // POST: /Account/Signup
        [HttpPost]
        public async Task<IActionResult> Signup(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            return View(usuario);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
