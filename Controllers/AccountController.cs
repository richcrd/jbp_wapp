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

namespace jbp_wapp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Json(roles);
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartamentos()
        {
            var departamentos = await _context.Departamentos.ToListAsync();
            return Json(departamentos);
        }

        [HttpGet]
        public async Task<IActionResult> GetGeneros()
        {
            var generos = await _context.Generos.ToListAsync();
            return Json(generos);
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup([FromBody] Usuario usuario)
        {
            Console.WriteLine($"Recibido: {JsonConvert.SerializeObject(usuario)}");

            if (usuario == null)
            {
                return BadRequest(new { Message = "Los datos del usuario son nulos" });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Message = "Datos inválidos", Errors = errors });
            }

            // Verificar si el correo ya está registrado
            var existingUser = await _context.Usuarios
                .AnyAsync(u => u.Correo == usuario.Correo);

            if (existingUser)
            {
                return BadRequest(new { Message = "El correo ya está registrado." });
            }
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Registro exitoso." });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == login.Correo && u.Contrasena == login.Contrasena);

            if (usuario != null)
            {
                HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
                HttpContext.Session.SetString("UsuarioNombre", usuario.NombreUsuario);
                return Ok(new { Message = "Inicio de sesion exitoso. "});
            }
            return Unauthorized("Correo o contrasena incorrectos");
        }
        //public async Task<IActionResult> Signup()
        //{
        //    // Cargar listas desde la base de datos
        //    await LoadViewBagData();
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Signup(TblUsuario us)
        //{
        //    var usuarioExistente = await _context.Tbl_Usuarios
        //        .FirstOrDefaultAsync(u => u.Usuario == us.Usuario || u.Correo == us.Correo);

        //    if (usuarioExistente != null)
        //    {
        //        ModelState.AddModelError(string.Empty, "El usuario o el correo ya están en uso.");
        //        return View(us);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Asignación de valores para las propiedades
        //        us.Fecha_Creacion = DateTime.Now;
        //        us.Fecha_Modificacion = DateTime.Now;
        //        us.IdEstado = 1; // Asegúrate de que este Id exista
        //        us.IdExperiencia = 1; // Asegúrate de que este Id exista
        //        us.IdProfesion = 1; // Asegúrate de que este Id exista

        //        try
        //        {
        //            _context.Tbl_Usuarios.Add(us);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("Login", "Account");
        //        }
        //        catch (DbUpdateException dbEx)
        //        {
        //            ModelState.AddModelError(string.Empty, "No se pudo guardar el usuario en la base de datos. Inténtalo de nuevo." + dbEx.Message);
        //            Console.WriteLine("No se pudo guardar el usuario en la base de datos. Inténtalo de nuevo." + dbEx);
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado: " + ex.Message);
        //            Console.WriteLine("Ocurrió un error inesperado: " + ex);
        //        }
        //    }

        //    // Si hay errores en el modelo, vuelve a cargar los datos del ViewBag
        //    await LoadViewBagData();
        //    return View(us);
        //}

        //public IActionResult Login()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(string correo, string contrasena)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Buscar el usuario en la base de datos
        //        var usuario = await _context.Tbl_Usuarios
        //            .FirstOrDefaultAsync(u => u.Correo == correo && u.Contrasena == contrasena);

        //        if (usuario != null)
        //        {
        //            // Guardar la sesión del usuario
        //            HttpContext.Session.SetInt32("UserId", usuario.Id);

        //            return RedirectToAction("Index", "Home");
        //        }

        //        ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
        //        return View();
        //    }

        //    return View();
        //}

        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear();
        //    return RedirectToAction("Login", "Account");
        //}

        //private async Task LoadViewBagData()
        //{
        //    ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Id", "Nombre");
        //    ViewBag.Departamentos = new SelectList(await _context.Departamentos.ToListAsync(), "Id", "Nombre");
        //    ViewBag.Generos = new SelectList(await _context.Generos.ToListAsync(), "Id", "Nombre");
        //}
    }
}
