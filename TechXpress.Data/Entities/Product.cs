using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using TechXpress.Data.ValueObjects;

namespace TechXpress.Data.Entities
{
    public class Product
    {
        public int Id { get;  set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }

        public string ImgUrl { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        
        public Category Category { get; set; } 

        public Product()
        {
            // Constructor for model binding
        }

        public Product(string name, decimal price, int categoryId, string imgUrl)
        {
            Name = name;
            Price = price;
            CategoryId = categoryId;
            ImgUrl = imgUrl;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Price must be greater than zero");
            Price = newPrice;
        }
    }
}