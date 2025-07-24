using System.ComponentModel.DataAnnotations;

namespace TameShop.ViewModels
{
    public class CartItemUpdateDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        public int AnimalId { get; set; }
    }
}
