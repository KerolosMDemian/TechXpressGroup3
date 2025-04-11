using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using TechXpress.Data.Entities;
using TechXpress.Data.RepositoriesInterfaces;
using TechXpress.Services.DTOs;
using TechXpress.Web.Models;

namespace TechXpress.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository<Product> _productRepository;
        private readonly IProductRepository<Category> _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _imageUploadPath;

        public ProductController(
            IProductRepository<Product> productRepository,
            IProductRepository<Category> categoryRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
            _imageUploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "imgs");

            
            if (!Directory.Exists(_imageUploadPath))
            {
                Directory.CreateDirectory(_imageUploadPath);
            }
        }

        public IActionResult Index()
        {
            var products = _productRepository.GetAllProduct();
            return View(products);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
                return NotFound();

            return View(product);

        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_categoryRepository.GetAllProduct(), "Id", "Name");
            return View(new ProductViewModel());
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken] // إضافة رمز منع تزوير الطلبات
        public async Task<IActionResult> Create(ProductViewModel productVM)
        {
            ViewBag.Categories = new SelectList(_categoryRepository.GetAllProduct(), "Id", "Name", productVM.Product.CategoryId);
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            try
            {
                string imageUrl = "default-product.png"; 

                if (productVM.Images != null)
                {
                    var imageFile = productVM.Images.FirstOrDefault();
                    imageUrl = await UploadFileAsync(imageFile);
                }

                

                var product = new Product(
                    productVM.Product.Name,
                    productVM.Product.Price,
                    productVM.Product.CategoryId,
                    imageUrl
                )
                {
                    Description = productVM.Product.Description,
                    StockQuantity = productVM.Product.StockQuantity
                };

                var categoryExists = _categoryRepository.GetAllProduct()
                    .Any(c => c.Id == productVM.Product.CategoryId);

                if (!categoryExists)
                {
                    ModelState.AddModelError("Product.CategoryId", "الفئة المحددة غير موجودة");
                    return View(productVM);
                }

                  await  _productRepository.Add(product);
                   await  _productRepository.Save(); 

                TempData["success"] = "product creation is done success!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error with create: {ex.Message}";
                return View(productVM);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
                return NotFound();

            // تحويل يدوي من Product إلى ProductDTO
            var productDTO = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
               
                CategoryId = product.CategoryId
            };

            var productVM = new ProductViewModel
            {
                Product = productDTO
            };

            ViewBag.Categories = new SelectList(_categoryRepository.GetAllProduct(), "Id", "Name", productDTO.CategoryId);
            return View(productVM);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel productVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_categoryRepository.GetAllProduct(), "Id", "Name", productVM.Product.CategoryId);
                return View(productVM);
            }

            var productToUpdate = _productRepository.GetProductById(productVM.Product.Id);
            if (productToUpdate == null)
                return NotFound();

            // تحديث البيانات
            productToUpdate.Name = productVM.Product.Name;
            productToUpdate.Description = productVM.Product.Description;
            productToUpdate.Price = productVM.Product.Price;
            productToUpdate.StockQuantity = productVM.Product.StockQuantity;
            productToUpdate.CategoryId = productVM.Product.CategoryId; // ربط الفئة بالمنتج
            ;

            // لو فيه صورة جديدة اترفعت، حدثها (من غير حذف القديمة)
            if (productVM.Images != null && productVM.Images.Any())
            {
                var imageFile = productVM.Images.FirstOrDefault();
                if (imageFile != null && imageFile.Length > 0)
                {
                    string imageUrl = await UploadFileAsync(imageFile);
                    productToUpdate.ImgUrl = imageUrl;
                }
            }

            _productRepository.Update(productToUpdate);
            await _productRepository.Save();

            TempData["success"] = "Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null) return NotFound();
            return View(product);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null) return NotFound();

            _productRepository.Delete(product);
            _productRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> UploadFileAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("invalid image file");
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string filePath = Path.Combine(_imageUploadPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return fileName;
        }
    }
}