

namespace TechXpress.Data.ValueObjects
{
    public class Stock
    {
        public int Id { get; set; }
        public int Quantity { get; private set; }
        public void UpdateStock(int quantity)
        {
            if (quantity < 0) throw new ArgumentException("Quantity Can Not Be Negative");
            Quantity = quantity;
        }
    }
}
