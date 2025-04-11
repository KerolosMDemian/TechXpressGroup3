

namespace TechXpress.Data.Entities
{
    public class Category
    {
        public int Id { get; private set; }
        public string? Name { get;  set; } 
        public List<Product> Products { get;  set; } = new();

        public Category()
        {
            
        }
    }
}
