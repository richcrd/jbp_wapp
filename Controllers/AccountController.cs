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
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Signup()
        {
            // Si el usuario ya esta autenticado
            if (User.Identity != null && User.Identity.IsAuthenticated) 
            {
                return RedirectToAction("Index", "Home");
            }
            await CargarDatos();
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            // Si el usuario ya esta autenticado
            if (User.Identity != null && User.Identity.IsAuthenticated) 
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(Usuario usuario)
        {
            if (usuario == null)
            {
                await CargarDatos();
                ViewBag.ErrorMessage = "Los datos del usuario son nulos";
                return View();
            }

            // poner limite de registro
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == usuario.NombreUsuario || u.Correo == usuario.Correo);

            if (usuarioExistente != null)
            {
                await CargarDatos();
                ViewBag.ErrorMessage = "El correo o usuario ya ha sido tomado";
                return View();
            }

            if (!ModelState.IsValid)
            {
                await CargarDatos();
                return View();
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Correo, string Contrasena)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Correo == Correo && u.Contrasena == Contrasena)
                .Select(u => new {
                    u.Id,
                    u.NombreUsuario,
                    Rol = u.IdRol
                })
                .FirstOrDefaultAsync();
            
            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Role, usuario.Rol.ToString())
                };
                 // Imprimir el rol del usuario en la consola
                Console.WriteLine($"Rol del usuario: {usuario.Rol}");
                // Crear la identidad y el principal del usuario
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Mantener la sesión activa entre cierres de navegador
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // Duración de la sesión
                };

                // Autenticar el usuario
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Correo o contraseña incorrectos.";
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // Cache de datos para evitar consultas repetitivas
        public async Task CargarDatos()
        {
            ViewBag.Roles = await _context.Roles
            .Where(r => r.Nombre != "admin")
            .ToListAsync();
            ViewBag.Departamentos = await _context.Departamentos.ToListAsync();
            ViewBag.Generos = await _context.Generos.ToListAsync();
        }
    }
}
