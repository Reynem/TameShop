namespace TameShop.ViewModels
{
    public class OrderItemResponseDTO
    {
        public int AnimalId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public required string AnimalName { get; set; }
    }
}
