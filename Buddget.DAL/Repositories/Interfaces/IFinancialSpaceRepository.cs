using Buddget.DAL.Entities;

namespace Buddget.DAL.Repositories.Interfaces
{
    public interface IFinancialSpaceRepository : IRepository<FinancialSpaceEntity>
    {
        Task<IEnumerable<FinancialSpaceEntity>> GetSpacesUserIsOwnerOfAsync(int userId);
        Task<IEnumerable<FinancialSpaceEntity>> GetSpacesUserIsMemberOfAsync(int userId);
        Task<IEnumerable<FinancialSpaceEntity>> GetSpacesUserIsMemberOrOwnerOf(int userId);
        Task<IEnumerable<FinancialSpaceEntity>> GetSpacesUserIsBannedIn(int categoryId);
        Task<FinancialSpaceEntity> GetFinancialSpaceAsync(int id);
    }
}