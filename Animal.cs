namespace TameShop
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Cost { get; set; } = 0.0;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public byte[]? ImageData { get; set; } = null;
    }
}
