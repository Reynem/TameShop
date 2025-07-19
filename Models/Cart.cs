using System.ComponentModel.DataAnnotations;

namespace TameShop.Models
{
    public class Cart
    {
        [Key]
        public string CartId { get; set; } = Guid.NewGuid().ToString();
        public required string UserId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Cart() { }

    }
}
