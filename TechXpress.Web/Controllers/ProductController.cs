using Microsoft.AspNetCore.Mvc;
using TechXpress.Data.Entities;
using TechXpress.Data.Repositories;

namespace TechXpress.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // جلب جميع المنتجات
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        // جلب منتج معين حسب الـ ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // إضافة منتج جديد
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (product == null) return BadRequest("Invalid product data.");

            await _productRepository.AddAsync(product);
            await _productRepository.SaveAsync();
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // تحديث بيانات المنتج
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null) return NotFound();

            existingProduct.UpdatePrice(updatedProduct.Price);
            _productRepository.Update(existingProduct);
            await _productRepository.SaveAsync();
            return NoContent();
        }

        // حذف المنتج
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            _productRepository.Delete(product);
            await _productRepository.SaveAsync();
            return NoContent();
        }
    }
}
