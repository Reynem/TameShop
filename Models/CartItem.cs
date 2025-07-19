namespace TameShop.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public int Quantity { get; set; } = 1;
    }
}
