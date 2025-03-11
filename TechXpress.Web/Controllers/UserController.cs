using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TechXpress.Data.Entities;
using TechXpress.Data.ValueObjects;

namespace TechXpress.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
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
    }
}
