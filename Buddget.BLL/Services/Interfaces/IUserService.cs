namespace Buddget.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> UserExistsAsync(int userId);
    }
}
