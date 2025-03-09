using System.ComponentModel.DataAnnotations;

namespace Buddget.DAL.Entities
{
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 100 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 100 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password hash is required.")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(user|admin)$", ErrorMessage = "Role must be either 'user' or 'admin'.")]
        public string Role { get; set; }

        public DateTime RegisteredAt { get; set; }

        public ICollection<CategoryEntity> Categories { get; set; }
        public ICollection<FinancialSpaceEntity> OwnedSpaces { get; set; }
        public ICollection<FinancialSpaceMemberEntity> SpaceMemberships { get; set; }
        public ICollection<TransactionEntity> Transactions { get; set; }
        public ICollection<FinancialGoalEntity> FinancialGoals { get; set; }
    }
}