using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Index(string userId)
        {
           userId = User.Identity.Name;
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                return View("EmptyCart"); // عرض صفحة سلة فارغة

            return View(cart);
        }

        [Authorize(Roles = "Customer")]       
        public async Task<IActionResult> AddToCart(string userId, int productId, int quantity)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                                           .ThenInclude(ci => ci.Product)
                                           .FirstOrDefaultAsync(c => c.UserId == userId);

            // إذا كانت السلة غير موجودة للمستخدم، نقوم بإنشائها
            if (cart == null)
            {
                cart = new Cart(userId);
                _context.Carts.Add(cart);
            }

            // البحث عن المنتج
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound("Product not found");

            // إذا كان المنتج موجودًا في السلة، نقوم بتحديث الكمية
            var existingItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;  // تحديث الكمية إذا كان المنتج موجودًا بالفعل
            }
            else
            {
                // إضافة عنصر جديد إذا لم يكن موجودًا في السلة
                var cartItem = new CartItem(productId, quantity);
                cart.AddItem(cartItem);
            }

            // حفظ التعديلات
            await _context.SaveChangesAsync();

            // إعادة توجيه المستخدم إلى صفحة السلة
            return RedirectToAction("Index", new { userId });
        }

        // مسح محتويات السلة
        public async Task<IActionResult> ClearCart(string userId)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                                           .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items);  // إزالة كل العناصر من السلة
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { userId });
        }

        // حذف عنصر من السلة
        public async Task<IActionResult> RemoveItem(string userId, int productId)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                                           .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);  // إزالة العنصر من السلة
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", new { userId });
        }
        public async Task<IActionResult> UpdateQuantity(string userId, int productId, int quantity)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                                           .ThenInclude(ci => ci.Product)
                                           .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;  // تحديث الكمية
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", new { userId });
        }

    }
}