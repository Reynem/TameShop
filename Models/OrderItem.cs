namespace TameShop.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int AnimalId { get; set; }
        public decimal Price { get; set; }
        public required string AnimalName { get; set; }
        public int Quantity { get; set; }
        
    }
}
