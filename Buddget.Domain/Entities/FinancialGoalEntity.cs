using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buddget.Domain.Entities
{
    public class FinancialGoalEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Goal name is required.")]
        [StringLength(255, ErrorMessage = "Goal name must be between 1 and 255 characters.", MinimumLength = 1)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Goal type is required.")]
        [RegularExpression("^(income|expense)$", ErrorMessage = "Type must be either 'income' or 'expense'.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Target amount is required.")]
        [Column(TypeName = "decimal(15,3)")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Target amount must be greater than 0.")]
        public decimal TargetAmount { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }

        public UserEntity User { get; set; }
        public ICollection<FinancialGoalCategoryEntity> Categories { get; set; } = new List<FinancialGoalCategoryEntity>();
        public ICollection<FinancialGoalSpaceEntity> Spaces { get; set; } = new List<FinancialGoalSpaceEntity>();
    }
}