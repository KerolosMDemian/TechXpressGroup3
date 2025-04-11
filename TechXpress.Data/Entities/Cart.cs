
namespace TechXpress.Data.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; private set; }
        public List<CartItem> Items { get; set; } = new();
        public Cart(string userId)
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
        public int ProductId { get;  set; }
        public int Quantity { get;  set; }
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public CartItem(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }


    }
}
