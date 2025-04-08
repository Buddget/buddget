namespace Buddget.BLL.DTOs
{
    public class CreateFinancialSpaceMemberDto
    {
        public int UserId { get; set; }
        public int FinancialSpaceId { get; set; }
        public string Role { get; set; }
    }
} 