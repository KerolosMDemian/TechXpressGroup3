using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.Data.Entities;
using TechXpress.Data.RepositoriesInterfaces;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly UserManager<User> _userManager;

        public CartController(ICartRepository cartRepo, IRepository<Product> productRepo, UserManager<User> userManager)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var cart = await _cartRepo.GetUserCartWithItemsAsync(userId);

            if (cart == null || !cart.Items.Any())
                return View("EmptyCart");

            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (quantity < 1)
                quantity = 1;

            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var cart = await _cartRepo.GetUserCartWithItemsAsync(userId);
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
                return NotFound("Product not found");

            if (cart == null)
            {
                cart = new Cart(userId);
                await _cartRepo.AddAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem(productId, quantity);
                cart.AddItem(cartItem);
            }

            await _cartRepo.SaveAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var cart = await _cartRepo.GetUserCartWithItemsAsync(userId);
            if (cart != null)
            {
                cart.Items.Clear();
                _cartRepo.Update(cart);
                await _cartRepo.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveItem(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var cart = await _cartRepo.GetUserCartWithItemsAsync(userId);
            var item = cart?.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Items.Remove(item);
                _cartRepo.Update(cart);
                await _cartRepo.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            if (quantity < 1)
                quantity = 1;

            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var cart = await _cartRepo.GetUserCartWithItemsAsync(userId);
            var item = cart?.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                _cartRepo.Update(cart);
                await _cartRepo.SaveAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
