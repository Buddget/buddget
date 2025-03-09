using System.ComponentModel.DataAnnotations;

namespace Buddget.DAL.Entities
{
    public class FinancialSpaceEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Space name is required.")]
        [StringLength(255, ErrorMessage = "Space name cannot exceed 255 characters.")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string Description { get; set; }

        [StringLength(255, ErrorMessage = "Image name cannot exceed 255 characters.")]
        public string ImageName { get; set; }

        public byte[] ImageData { get; set; }

        [Required(ErrorMessage = "Owner ID is required.")]
        public int OwnerId { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserEntity Owner { get; set; }
        public ICollection<FinancialSpaceMemberEntity> Members { get; set; }
        public ICollection<TransactionEntity> Transactions { get; set; }
        public ICollection<FinancialGoalSpaceEntity> FinancialGoalSpaces { get; set; }
    }
}