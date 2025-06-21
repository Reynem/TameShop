namespace TameShop.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BirthTime { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public byte[]? ProfilePicture { get; set; } = null;
    }
}
