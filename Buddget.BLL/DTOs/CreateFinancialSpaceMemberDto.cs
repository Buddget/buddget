namespace Buddget.BLL.DTOs
{
    public class CreateFinancialSpaceMemberDto
    {
        public int FinancialSpaceId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; }
    }
}