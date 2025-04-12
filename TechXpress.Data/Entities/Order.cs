

namespace TechXpress.Data.Entities
{
    public class Order
    {
        public int Id { get; private set; }
        public string UserId { get; private set; }
        public User User { get; private set; } = null!;
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; private set; } = new();
        public Order(string userId)
        {
            UserId = userId;
            OrderDate = DateTime.Now;
        }
        public void AddItem(OrderItem item)
        {
            if(item == null) throw new ArgumentNullException(nameof(item));
            Items.Add(item);
        }
    }

   public class OrderItem
   {
    public int OrderId { get;  set; }  // إضافة OrderId
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Product Product { get; private set; } = null!;
   public Order Order { get; private set; } = null!;  // خاصية الـ Navigation Property


        public OrderItem(int orderId, int productId, int quantity)
        {
        OrderId = orderId;  // ربط OrderItem بالـ Order
        ProductId = productId;
        Quantity = quantity;
        }
   }
}

