using Buddget.BLL.DTOs;
public class EditTransactionViewModel
{
    public TransactionDto Transaction { get; set; }
    public IEnumerable<CategoryDto> Categories { get; set; }
}