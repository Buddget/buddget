using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buddget.DAL.Entities
{
    public class TransactionEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Financial space ID is required.")]
        public int FinancialSpaceId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Transaction name is required.")]
        [StringLength(255, ErrorMessage = "Transaction name cannot exceed 255 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Transaction amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        [Column(TypeName = "decimal(15,3)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        [StringLength(10, ErrorMessage = "Currency code cannot exceed 10 characters.")]
        public string Currency { get; set; }

        [Required(ErrorMessage = "Transaction type is required.")]
        [RegularExpression("^(income|expense)$", ErrorMessage = "Type must be either 'income' or 'expense'.")]
        public string Type { get; set; }

        public int? CategoryId { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Transaction date is required.")]
        public DateTime Date { get; set; }
        public FinancialSpaceEntity FinancialSpace { get; set; }
        public UserEntity User { get; set; }
        public CategoryEntity? Category { get; set; }
    }
}