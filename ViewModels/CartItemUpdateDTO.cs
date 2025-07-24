using System.ComponentModel.DataAnnotations;

namespace TameShop.ViewModels
{
    public class CartItemUpdateDTO
    {
        [Required]
        public int AnimalId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
