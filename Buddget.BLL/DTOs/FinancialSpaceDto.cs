namespace Buddget.BLL.DTOs
{
    public class FinancialSpaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageData { get; set; }
        public string OwnerName { get; set; }
        public string OwnerLastName { get; set; }
        public List<FinancialGoalDto> Goals { get; set; } = new();
        public List<FinancialSpaceMemberDto> Members { get; set; } = new();
        public List<TransactionDto> RecentTransactions { get; set; } = new();
    }
}
