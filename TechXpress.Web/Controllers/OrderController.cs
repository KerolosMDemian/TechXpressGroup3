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
using TechXpress.Data.UnitOfWork;


namespace TechXpress.Controllers
{
   
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository<Product> _productRepo;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IOrderRepository orderRepo,
                                ICartRepository cartRepo,
                                IProductRepository<Product> productRepo,
                               UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
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
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var cart = await _cartRepo.GetUserCartWithItemsAsync(user.Id);

                if (cart == null || !cart.Items.Any())
                {
                    TempData["Error"] = "Your cart is empty. Cannot place the order.";
                    return RedirectToAction("Index", "Cart");
                }

                var order = new Order(user.Id);

                foreach (var item in cart.Items)
                {
                    var product =  _productRepo.GetProductById(item.ProductId);

                    if (product == null)
                    {
                        TempData["Error"] = $"Product with ID {item.ProductId} does not exist.";
                        return RedirectToAction("Index", "Cart");
                    }

                    if (product.StockQuantity < item.Quantity)
                    {
                        TempData["Error"] = $"Insufficient stock for product '{product.Name}'. Only {product.StockQuantity} item(s) available.";
                        return RedirectToAction("Index", "Cart");
                    }

                    
                    product.StockQuantity -= item.Quantity;
                    _productRepo.Update(product);

                    var orderItem = new OrderItem(order.Id, item.ProductId, item.Quantity);
                    order.AddItem(orderItem);
                }

                await _orderRepo.AddAsync(order);

                
                cart.Items.Clear();
                _cartRepo.Update(cart);

                await _unitOfWork.SaveChangesAsync();

                TempData["Success"] = "Your order has been placed successfully!";
                return RedirectToAction("Confirmation", new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                // Log the error or send it to a logging system
                TempData["Error"] = "An error occurred while processing your order. Please try again.";
                return RedirectToAction("Index", "Cart");
            }
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
