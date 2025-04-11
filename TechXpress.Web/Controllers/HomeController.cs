using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;
using TechXpress.Data.RepositoriesInterfaces;
using TechXpress.Web.Models;

namespace TechXpress.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;  // ≈÷«›… «·‹ DbContext ··Ê’Ê· ≈·Ï «·»Ì«‰« 

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.Identity.Name;
            // «” —Ã«⁄ «·›∆«  „⁄ «·„‰ Ã«  «·„— »ÿ… »Â« „‰ ﬁ«⁄œ… «·»Ì«‰« 
            var categoriesWithProducts = _context.Categories
                                                 .Include(c => c.Products)  // «· √ﬂœ „‰  Õ„Ì· «·„‰ Ã«  „⁄ «·›∆« 
                                                 .ToList();
            return View(categoriesWithProducts);  // ≈—”«· «·»Ì«‰«  ≈·Ï «·⁄—÷
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
