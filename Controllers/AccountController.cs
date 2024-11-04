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
            if (User.Identity.IsAuthenticated) 
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
            if (User.Identity.IsAuthenticated) 
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
                Console.WriteLine($"Estableciendo UsuarioId en sesión: {usuario.Id}");
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("UsuarioNombre", usuario.NombreUsuario);
                HttpContext.Session.SetInt32("UsuarioRol", usuario.Rol);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Correo o contraseña incorrectos.";
            return View();
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UsuarioId");
            return RedirectToAction("Login", "Account");
        }

        // Cache de datos para evitar consultas repetitivas
        public async Task CargarDatos()
        {
            ViewBag.Roles = await _context.Roles.ToListAsync();
            ViewBag.Departamentos = await _context.Departamentos.ToListAsync();
            ViewBag.Generos = await _context.Generos.ToListAsync();
        }
    }
}
