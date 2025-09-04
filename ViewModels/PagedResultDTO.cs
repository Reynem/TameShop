using System.ComponentModel.DataAnnotations;

namespace TameShop.ViewModels
{
    public class PagedResultDTO<T>
    {
        [Required]
        public required int TotalCount { get; set; }
        [Required]
        public required int Page {  get; set; }
        [Required]
        public required int PageSize { get; set; }
        [Required]
        public required IEnumerable<T> Data { get; set; }
    }
}
