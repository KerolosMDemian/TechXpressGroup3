using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;
using TechXpress.Data.RepositoriesInterfaces;
using TechXpress.Web.Models;
using System.Linq;


namespace TechXpress.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;  // ≈÷«›… «·‹ DbContext ··Ê’Ê· ≈·Ï «·»Ì«‰« 

        public HomeController(ILogger<HomeController> logger, AppDbContext context
            , IRepository<User> userRepository, IRepository<Product> productRepository,
            IRepository<Category> categoryRepository, IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _context = context;
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.UsersCount = _userRepository.GetAll().Count();
                ViewBag.ProductsCount = _productRepository.GetAll().Count();
                ViewBag.CategoriesCount = _categoryRepository.GetAll().Count();
                ViewBag.OrdersCount = _orderRepository.GetAll().Count();

            }
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
