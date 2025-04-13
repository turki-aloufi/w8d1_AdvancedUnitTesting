using w8d1_AdvancedUnitTesting.Models;

namespace w8d1_AdvancedUnitTesting.Services
{
    public interface IOrderRepository
    {
        Task CreateAsync(Order order);
        Task<Order> GetByIdAsync(int orderId);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int orderId);
    }
}
