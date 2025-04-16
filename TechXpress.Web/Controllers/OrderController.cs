using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;
using TechXpress.Data.Enums;
using TechXpress.Data.Repositories;
using TechXpress.Data.RepositoriesInterfaces;
using TechXpress.Data.UnitOfWork;
using TechXpress.Services.Interfaces;


namespace TechXpress.Controllers
{
   
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository<Product> _productRepo;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeService _stripeService;

        public OrderController(IOrderRepository orderRepo,
                                ICartRepository cartRepo,
                                IProductRepository<Product> productRepo,
                               UserManager<User> userManager, IUnitOfWork unitOfWork, IStripeService stripeService)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _stripeService = stripeService;
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

        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(PaymentMethod paymentMethod)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = await _cartRepo.GetUserCartWithItemsAsync(user.Id);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            if (paymentMethod == PaymentMethod.Stripe)
            {
                // نعمل جلسة الدفع فقط
                var session = await _stripeService.CreateCheckoutSessionAsync(user.Id, cart);
                return Redirect(session.Url);
            }
            else if (paymentMethod == PaymentMethod.COD)
            {
                // هنا نسجل الأوردر فورًا ونخصم الستوك
                return await FinalizeOrder(user.Id, cart, PaymentMethod.COD, isPaid: true);
            }

            TempData["Error"] = "Invalid payment method.";
            return RedirectToAction("Index", "Cart");
        }

        private async Task<IActionResult> FinalizeOrder(string userId, Cart cart, PaymentMethod method, bool isPaid, string stripeSessionId = null)
        {
            using var transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();

            try
            {
                // تأكد من أن stripeSessionId موجود فقط إذا كانت طريقة الدفع Stripe
                if (method == PaymentMethod.Stripe && string.IsNullOrEmpty(stripeSessionId))
                {
                    TempData["Error"] = "Stripe session ID is missing.";
                    return RedirectToAction("Index", "Cart");
                }

                var order = new Order(userId, method)
                {
                    IsPaid = isPaid,
                    StripeSessionId = stripeSessionId,
                   
                    OrderStatus = method == PaymentMethod.COD ? OrderStatus.Pending : OrderStatus.Processing // تغيير الحالة هنا

                };

                foreach (var item in cart.Items)
                {
                    var product = _productRepo.GetProductById(item.ProductId);
                    if (product == null || product.StockQuantity < item.Quantity)
                    {
                        TempData["Error"] = $"Product {item.ProductId} is not available.";
                        return RedirectToAction("Index", "Cart");
                    }

                    product.StockQuantity -= item.Quantity;
                    _productRepo.Update(product);

                    order.AddItem(new OrderItem(order.Id, item.ProductId, item.Quantity));
                }

                await _orderRepo.AddAsync(order);
                cart.Items.Clear();
                _cartRepo.Update(cart);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                // التأكد من التعامل مع COD أو Stripe بشكل صحيح
                if (method == PaymentMethod.COD)
                {
                    // في حالة COD يمكننا توجيه المستخدم مباشرة إلى التأكيد
                    return RedirectToAction("Confirmation", new { orderId = order.Id });
                }
                else if (method == PaymentMethod.Stripe)
                {
                    return RedirectToAction("Confirmation", new { orderId = order.Id });

                }

                TempData["Error"] = "Invalid payment method.";
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Error finalizing order: " + ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpGet]
        public async Task<IActionResult> StripeSuccess(string session_id)
        {
            var session = await _stripeService.GetSessionDetailsAsync(session_id);
            var userId = session.Metadata["UserId"];

            var cart = await _cartRepo.GetUserCartWithItemsAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            return await FinalizeOrder(userId, cart, PaymentMethod.Stripe, isPaid: true, stripeSessionId: session_id);
        }





        public async Task<IActionResult> Confirmation(int orderId)
        {
            var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return NotFound();

            // في حالة COD، نعتبر أن الدفع تم مباشرة
            if (order.PaymentMethod == PaymentMethod.COD)
            {
                order.PaymentStatus = PaymentStatus.Paid;
            }
            // في حالة الدفع باستخدام Stripe
            else if (order.PaymentMethod == PaymentMethod.Stripe)
            {
                // تحقق من حالة الدفع باستخدام Stripe Session ID
                var paymentStatus = await _stripeService.GetPaymentStatusAsync(order.StripeSessionId);

                if (paymentStatus == "paid")
                {
                    order.PaymentStatus = PaymentStatus.Paid;
                    order.OrderStatus = OrderStatus.Processing;
                }
                else if (paymentStatus == "unpaid" || paymentStatus == "expired")
                {
                    order.PaymentStatus = PaymentStatus.Failed;
                    order.OrderStatus = OrderStatus.Cancelled;
                }
                else
                {
                    // حالة غير متوقعة
                    order.PaymentStatus = PaymentStatus.Failed;
                    order.OrderStatus = OrderStatus.Cancelled;
                }
            }

            // حفظ التغييرات في قاعدة البيانات
            await _unitOfWork.SaveChangesAsync();

            // إعادة التوجيه إلى الصفحة التي تعرض تفاصيل الأوردر
            return View(order);
        }


    }

}
