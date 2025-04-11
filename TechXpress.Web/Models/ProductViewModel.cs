using TechXpress.Services.DTOs;

namespace TechXpress.Web.Models
{
    public class ProductViewModel
    {
        public ProductDTO Product { get; set; } = new ProductDTO();
        public IEnumerable<IFormFile>? Images { get; set; }
    }
}