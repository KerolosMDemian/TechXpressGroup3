using TechXpress.Data.Enums;

namespace TechXpress.Data.Entities
{
    public class Order
    {
        public int Id { get; private set; }
        public string UserId { get; private set; }
        public User User { get; private set; } = null!;
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; private set; } = new();
        public string? StripeSessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; }

        public Order(string userId, PaymentMethod paymentMethod)
        {
            UserId = userId;
            PaymentMethod = paymentMethod;
            OrderDate = DateTime.Now;
        }
        public void AddItem(OrderItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Items.Add(item);
        }
    }

    public class OrderItem
    {
        public int OrderId { get; set; }  
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; } = null!;
        public Order Order { get; set; } = null!;

        public OrderItem(int orderId, int productId, int quantity)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
