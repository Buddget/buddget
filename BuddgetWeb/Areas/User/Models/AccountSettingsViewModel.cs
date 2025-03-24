using Buddget.BLL.DTOs;

namespace BuddgetWeb.Areas.User.Models
{
    public class AccountSettingsViewModel
    {
        public UserDto User { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; }
    }
}
