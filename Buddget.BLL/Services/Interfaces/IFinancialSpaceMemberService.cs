﻿using Buddget.BLL.DTOs;

namespace Buddget.BLL.Services.Interfaces
{
    public interface IFinancialSpaceMemberService
    {
        Task<IEnumerable<FinancialSpaceMemberDto>> GetMembersBySpaceIdAsync(int spaceId);
        Task<IEnumerable<FinancialSpaceMemberDto>> GetBannedMembersBySpaceIdAsync(int spaceId);
    }
}
