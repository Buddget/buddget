using Buddget.BLL.DTOs;

namespace BuddgetWeb.Areas.User.Models
{
    public class MySpacesViewModel
    {
        public List<FinancialSpaceDto> FinancialSpaces { get; set; } = new List<FinancialSpaceDto>();
    }
}