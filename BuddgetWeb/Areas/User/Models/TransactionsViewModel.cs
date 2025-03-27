using Buddget.BLL.DTOs;
using Buddget.BLL.Enums;

public class TransactionsViewModel
{
    public IEnumerable<TransactionDto> Transactions { get; set; }
    public int FinancialSpaceId { get; set; }
    public string FinancialSpaceName { get; set; }
    public string FinancialSpaceOwner { get; set; }
    public TransactionSortColumnEnum SortColumn { get; set; }
    public bool Ascending { get; set; }
    public IEnumerable<FinancialSpaceDto> UserSpaces { get; set; }
}