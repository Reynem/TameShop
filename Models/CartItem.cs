using System.ComponentModel.DataAnnotations;

namespace TameShop.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public required string CartId { get; set; }

        public int Quantity { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int AnimalId { get; set; }
        public virtual Cart Cart { get; set; } = null!; 
        public virtual Animal Animal { get; set; } = null!;
    }
}
