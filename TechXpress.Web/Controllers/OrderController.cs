using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;

namespace TechXpress.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // عرض جميع الطلبات لمستخدم معين
        public async Task<IActionResult> Index(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return View(orders);
        }

        // عرض تفاصيل الطلب
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // إنشاء طلب جديد
        [HttpPost]
        public async Task<IActionResult> Create(int userId)
        {
            var order = new Order(userId);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { userId });
        }
    }
}
