using System.ComponentModel.DataAnnotations;

namespace Buddget.DAL.Entities
{
    public class FinancialGoalSpaceEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Financial goal ID is required.")]
        public int FinancialGoalId { get; set; }
        
        [Required(ErrorMessage = "Financial space ID is required.")]
        public int FinancialSpaceId { get; set; }
        
        public FinancialGoalEntity FinancialGoal { get; set; }
        public FinancialSpaceEntity FinancialSpace { get; set; }
    }
}