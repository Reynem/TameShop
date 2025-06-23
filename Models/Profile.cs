using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TameShop.Models
{
    public class Profile
    {
        [Key]
        [ForeignKey("User")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public byte[]? ProfilePicture { get; set; } = null;

        public virtual User User { get; set; } = null!;
    }
}
