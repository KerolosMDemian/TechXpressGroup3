using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;
using TechXpress.Data.RepositoriesInterfaces;

namespace TechXpress.Controllers
{
    
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _repository;
        private readonly IProductRepository<Product> _productRepository;

        public CategoryController(IRepository<Category> repository, IProductRepository<Product> productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        
        public  IActionResult Index()
        {
            var categories =  _repository.GetAll(); // Await the task to get the data
            return View(categories);
        }
        
       
       
        public async Task<ActionResult> Details(int id)
        {
             var category = await _repository.GetByIdAsync(id);

            if (category == null) return NotFound();
            var products = _productRepository.GetAllProduct();
            return View(category);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(category);  // انتظار العملية بشكل صحيح
                await _repository.SaveAsync();         // انتظار عملية الحفظ
                return RedirectToAction(nameof(Index)); // إعادة التوجيه إلى صفحة Index بعد الإضافة
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                _repository.Update(category);
                await _repository.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return NotFound();
            _repository.Delete(category);
            await _repository.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
