using System.ComponentModel.DataAnnotations;

namespace Buddget.DAL.Entities
{
    public class FinancialGoalCategoryEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Financial goal ID is required.")]
        public int FinancialGoalId { get; set; }
        
        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }
        
        public FinancialGoalEntity FinancialGoal { get; set; }
        public CategoryEntity Category { get; set; }
    }
}