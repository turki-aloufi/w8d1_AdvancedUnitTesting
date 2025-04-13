using w8d1_AdvancedUnitTesting.Models;

namespace w8d1_AdvancedUnitTesting.Services
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User> GetByIdAsync(int id);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}
