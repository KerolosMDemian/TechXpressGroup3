using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;
using TechXpress.Data.Repositories;
using TechXpress.Data.RepositoriesInterfaces;

namespace TechXpress.Controllers
{
   
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository<Product> _productRepo;
        private readonly UserManager<User> _userManager;

        public OrderController(IOrderRepository orderRepo,
                                ICartRepository cartRepo,
                                IProductRepository<Product> productRepo,
                               UserManager<User> userManager)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepo.GetAll()
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();

            return View(orders);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepo.GetAll()
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = await _cartRepo.GetUserCartWithItemsAsync(user.Id);

            if (cart == null || !cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            var order = new Order(user.Id);

            foreach (var item in cart.Items)
            {
                var product =  _productRepo.GetProductById(item.ProductId); // تزامن في استرجاع الـ Product
                if (product == null) continue;

                var orderItem = new OrderItem(order.Id, item.ProductId, item.Quantity);  // إضافة OrderId هنا
                order.AddItem(orderItem);
            }

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveAsync(); // حفظ الطلب في قاعدة البيانات

            // مسح السلة بعد الطلب
            cart.Items.Clear();
            _cartRepo.Update(cart);
            await _cartRepo.SaveAsync(); // حفظ التغييرات في السلة

            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }



        public async Task<IActionResult> Confirmation(int orderId)
        {
            var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return NotFound();

            return View(order);
        }
    }

}
