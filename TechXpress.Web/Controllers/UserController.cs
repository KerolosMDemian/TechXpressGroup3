using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TechXpress.Data.Entities;
using TechXpress.Data.ValueObjects;
using TechXpress.Web.Models;
using TechXpress.Web.Controllers;

namespace TechXpress.Controllers
{
    public class UserController : Controller
    {
       private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

       
        public UserController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
           _signInManager = signInManager;
            _userManager = userManager;
        }

        // عرض بيانات المستخدم
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            return View(user);
        }

        // تعديل عنوان المستخدم
        [HttpPost]
        public async Task<IActionResult> UpdateAddress(int id, Address newAddress)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            user.UpdateAddress(newAddress);
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Details", new { id });
        }
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register( string name, string email, string password, string street, string city, string state, string country, string zipcode, string phonenumber)
        {
            var user = new User
            {
               Name = name,
                Email = email,
                Address = new Address(street,city,state,country,zipcode),
                UserName = email,
                PhoneNumber = phonenumber
            };


            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                TempData["SuccessMessage"] = "Your account has been created successfully. Please log in.";
               return RedirectToAction("Login", "User");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            // إذا كان returnUrl فارغًا، تعيين قيمة افتراضية
            returnUrl ??= Url.Content("~/");

            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email or Password.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
