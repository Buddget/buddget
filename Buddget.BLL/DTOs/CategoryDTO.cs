namespace Buddget.BLL.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
}