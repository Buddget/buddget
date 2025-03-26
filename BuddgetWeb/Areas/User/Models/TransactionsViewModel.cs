using Buddget.BLL.DTOs;

public class TransactionsViewModel
{
    public IEnumerable<TransactionDto> Transactions { get; set; }
    public int FinancialSpaceId { get; set; }
    public string FinancialSpaceName { get; set; }
    public string FinancialSpaceOwner { get; set; }
}