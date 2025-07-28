using System.ComponentModel.DataAnnotations;

namespace TameShop.ViewModels
{
    public class OrderItemDTO
    {
        [Required]
        public int AnimalId { get; set; }

        [Range(1, 99)]
        public int Quantity { get; set; } = 1;
    }
}
