using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Buddget.DAL.Entities
{
    public class UserEntity : IdentityUser<int>
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 100 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 100 characters.")]
        public string LastName { get; set; }

        public DateTime RegisteredAt { get; set; }

        public ICollection<CategoryEntity> Categories { get; set; }
        public ICollection<FinancialSpaceEntity> OwnedSpaces { get; set; }
        public ICollection<FinancialSpaceMemberEntity> SpaceMemberships { get; set; }
        public ICollection<TransactionEntity> Transactions { get; set; }
        public ICollection<FinancialGoalEntity> FinancialGoals { get; set; }
    }
}