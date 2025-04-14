using System.ComponentModel.DataAnnotations;

namespace Buddget.Domain.Entities
{
    public class CategoryEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name must be between 1 and 100 characters.", MinimumLength = 1)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category default status is required.")]
        public bool IsDefault { get; set; } = false;

        public UserEntity User { get; set; }
    }
}