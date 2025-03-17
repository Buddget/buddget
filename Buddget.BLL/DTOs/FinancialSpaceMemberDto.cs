using System;

namespace Buddget.BLL.DTOs
{
    public class FinancialSpaceMemberDto : UserDto
    {
        public int FinancialSpaceId { get; set; }
        public string MemberRole { get; set; }
    }
}
