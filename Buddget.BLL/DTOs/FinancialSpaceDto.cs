using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<UserDto> Members { get; set; } = new();
        public List<TransactionDto> RecentTransactions { get; set; } = new();
    }

    public class FinancialGoalDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal TargetAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastType { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime RegisteredAt { get; set; }

    }

    public class TransactionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public string CategoryName { get; set; }
    }
}
