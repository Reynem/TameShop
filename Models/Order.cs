namespace TameShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public required List<OrderItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed,
        Cancelled
    }
}
