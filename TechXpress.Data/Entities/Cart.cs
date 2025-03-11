
namespace TechXpress.Data.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; private set; }
        public List<CartItem> Items { get; private set; } = new();
        public Cart(int userId)
        {
            UserId = userId;
           
        }
        public void AddItem(CartItem item)
        {
            if(item == null) throw new ArgumentNullException(nameof(item));
            Items.Add(item);
        }
    }
    public class CartItem
    {
        public int CartId { get; set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public Cart Cart { get; private set; } = null!;
        public Product Product { get; private set; } = null!;
        public CartItem(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }


    }
}
