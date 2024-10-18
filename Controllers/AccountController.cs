using jbp_wapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace jbp_wapp.Controllers
{
    public class AccountController : Controller
    {
        private static List<UserModel> _userList = new List<UserModel>();

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
        public IActionResult Login(string username, string password)
        {
            var user = _userList.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = "Invalid credentials";
            return View();
        }

        // POST: /Account/Signup
        [HttpPost]
        public IActionResult Signup(string username, string email, string password)
        {
            if (_userList.Any(u => u.Username == username))
            {
                ViewBag.Message = "User already exists";
                return View();
            }
            _userList.Add(new UserModel { Username = username, Email = email, Password = password });
            return RedirectToAction("Login");
        }
    }
}
