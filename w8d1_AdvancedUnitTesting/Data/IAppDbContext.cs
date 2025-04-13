using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using w8d1_AdvancedUnitTesting.Models;

namespace w8d1_AdvancedUnitTesting.Services
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Order> Orders { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}