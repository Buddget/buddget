using System.ComponentModel.DataAnnotations;

namespace Buddget.DAL.Entities
{
    public class FinancialSpaceMemberEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Financial space ID is required.")]
        public int FinancialSpaceId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [StringLength(50, ErrorMessage = "Role cannot exceed 50 characters.")]
        public string Role { get; set; } // "owner", "member", or "banned"

        public FinancialSpaceEntity FinancialSpace { get; set; }
        public UserEntity User { get; set; }
    }
}