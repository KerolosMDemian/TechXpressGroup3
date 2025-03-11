

namespace TechXpress.Data.ValueObjects
{
    public class Price
    {
        public int Id { get; set; }
        public decimal Value { get; private set; }
        
        public Price(decimal price)
        {
            if (price <= 0) throw new ArgumentException("Price Must be Greater than Zero");
            Value = price;
        }
        public void UpdatePrice(decimal newPrice)
        {
            
            Value = newPrice;
        }
    }
}
