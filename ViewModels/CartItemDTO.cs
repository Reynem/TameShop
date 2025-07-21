namespace TameShop.ViewModels
{
    public class CartItemDTO
    {
        public int Quantity { get; set; }
        public int AnimalId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
