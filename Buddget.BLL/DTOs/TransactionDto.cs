namespace Buddget.BLL.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string AuthorName { get; set; }
        public string Type { get; set; }
    }
}