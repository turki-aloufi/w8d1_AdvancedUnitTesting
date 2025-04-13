using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using w8d1_AdvancedUnitTesting.Services;
using w8d1_AdvancedUnitTesting.Models;
using w8d1_AdvancedUnitTesting.Data;

namespace w8d1_AdvancedUnitTesting.Test
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private Mock<AppDbContext> _dbContextMock;
        private Mock<DbSet<User>> _userDbSetMock;
        private IUserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            _userDbSetMock = new Mock<DbSet<User>>();
            _dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _dbContextMock.Setup(c => c.Users).Returns(_userDbSetMock.Object);
            _userRepository = new UserRepository(_dbContextMock.Object);
        }

        [Test]
        public async Task CreateAsync_ValidUser_CallsAddAndSave()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            _userDbSetMock.Setup(m => m.AddAsync(user, It.IsAny<CancellationToken>())).ReturnsAsync((EntityEntry<User>)null);
            _dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _userRepository.CreateAsync(user);

            // Assert
            _userDbSetMock.Verify(m => m.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once());
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            _userDbSetMock.Setup(m => m.FindAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _userRepository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.FirstName, Is.EqualTo("John"));
        }

        [Test]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            _userDbSetMock.Setup(m => m.FindAsync(999)).ReturnsAsync((User)null);

            // Act
            var result = await _userRepository.GetByIdAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateAsync_ValidUser_CallsUpdateAndSave()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" };
            _userDbSetMock.Setup(m => m.Update(user));
            _dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _userRepository.UpdateAsync(user);

            // Assert
            _userDbSetMock.Verify(m => m.Update(user), Times.Once());
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteAsync_ExistingId_CallsRemoveAndSave()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            _userDbSetMock.Setup(m => m.FindAsync(1)).ReturnsAsync(user);
            _userDbSetMock.Setup(m => m.Remove(user));
            _dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _userRepository.DeleteAsync(1);

            // Assert
            _userDbSetMock.Verify(m => m.FindAsync(1), Times.Once());
            _userDbSetMock.Verify(m => m.Remove(user), Times.Once());
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteAsync_NonExistentId_DoesNotCallRemove()
        {
            // Arrange
            _userDbSetMock.Setup(m => m.FindAsync(999)).ReturnsAsync((User)null);

            // Act
            await _userRepository.DeleteAsync(999);

            // Assert
            _userDbSetMock.Verify(m => m.FindAsync(999), Times.Once());
            _userDbSetMock.Verify(m => m.Remove(It.IsAny<User>()), Times.Never());
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        [TestCase("john.doe@example.com")]
        public async Task GetByIdAsync_ValidEmail_ReturnsUser(string email)
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = email };
            _userDbSetMock.Setup(m => m.FindAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _userRepository.GetByIdAsync(1);

            // Assert
            Assert.That(result.Email, Is.EqualTo(email));
        }
    }
}