using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;

namespace TechXpress.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // عرض السلة للمستخدم
        public async Task<IActionResult> Index(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                return View("EmptyCart"); // عرض صفحة سلة فارغة

            return View(cart);
        }

        // إضافة منتج للسلة
        public async Task<IActionResult> AddToCart(int userId, int productId, int quantity)
        {
            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart(userId);
                _context.Carts.Add(cart);
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound("Product not found");

            var cartItem = new CartItem(productId, quantity);
            cart.AddItem(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { userId });
        }

        // مسح محتويات السلة
        public async Task<IActionResult> ClearCart(int userId)
        {
            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", new { userId });
        }
    }
}
