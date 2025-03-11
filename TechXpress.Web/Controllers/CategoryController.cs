using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;

namespace TechXpress.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // عرض كل الفئات
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // عرض تفاصيل الفئة ومنتجاتها
        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // إضافة فئة جديدة
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Category name cannot be empty.");

            var category = new Category(name);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
