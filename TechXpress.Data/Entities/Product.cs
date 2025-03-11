

using TechXpress.Data.ValueObjects;

namespace TechXpress.Data.Entities
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; }=string.Empty;
        public decimal Price { get; private set; }
        public Stock StockQuantity { get; private set; }=new Stock();
        public string ImgUrl { get; private set; } = string.Empty;
        public int CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;
        private Product() { }
        public Product(string name , decimal price , int categoryid , string imgUrl)
        {
            Price NewPrice = new Price(price);
            Name = name;
            Price = NewPrice.Value;
            CategoryId = categoryid;
            ImgUrl = imgUrl;
        }
        public Product(string name, decimal price, Stock stockQuantity, int categoryid, string imgUrl)
        : this(name, price, categoryid, imgUrl)
        {
            StockQuantity = stockQuantity;
        }
        

        public void UpdatePrice(decimal newPrice)
        {
            if (Price <= 0) throw new ArgumentException("Price Must be Greater than Zero");
            Price = newPrice;
        }



    }
}
