using CartItem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartItem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository userRepository;

        // Use dependency injection to get the UserRepository
        public AccountController(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid) // Validate the model
            {
                userRepository.RegisterUser(user);
                return RedirectToAction("Login");
            }

            return View(user); // Return the same view with validation errors
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = userRepository.Login(username, password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
    }
}
