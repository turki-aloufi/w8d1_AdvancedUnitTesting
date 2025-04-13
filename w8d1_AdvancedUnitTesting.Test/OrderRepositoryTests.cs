using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using w8d1_AdvancedUnitTesting.Data;
using w8d1_AdvancedUnitTesting.Models;
using w8d1_AdvancedUnitTesting.Services;

namespace UnitTests
{
    [TestFixture]
    public class OrderRepositoryTests
    {
        private Mock<AppDbContext> _dbContextMock;
        private Mock<DbSet<Order>> _orderDbSetMock;
        private IOrderRepository _orderRepository;

        [SetUp]
        public void Setup()
        {
            _orderDbSetMock = new Mock<DbSet<Order>>();
            _dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _dbContextMock.Setup(c => c.Orders).Returns(_orderDbSetMock.Object);
            _orderRepository = new OrderRepository(_dbContextMock.Object);
        }

        [Test]
        public async Task CreateAsync_ValidOrder_CallsAddAndSave()
        {
            // Arrange
            var order = new Order { OrderId = 1, UserId = 1, Product = "Laptop", Quantity = 2, Price = 999.99m };
            _orderDbSetMock.Setup(m => m.AddAsync(order, default)).ReturnsAsync((EntityEntry<Order>)null);
            _dbContextMock.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _orderRepository.CreateAsync(order);

            // Assert
            _orderDbSetMock.Verify(m => m.AddAsync(order, default), Times.Once());
            _dbContextMock.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsOrder()
        {
            // Arrange
            var order = new Order { OrderId = 1, UserId = 1, Product = "Laptop", Quantity = 2, Price = 999.99m };
            _orderDbSetMock.Setup(m => m.FindAsync(1)).ReturnsAsync(order);

            // Act
            var result = await _orderRepository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.OrderId, Is.EqualTo(1));
            Assert.That(result.Product, Is.EqualTo("Laptop"));
        }

        [Test]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            _orderDbSetMock.Setup(m => m.FindAsync(999)).ReturnsAsync((Order)null);

            // Act
            var result = await _orderRepository.GetByIdAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateAsync_ValidOrder_CallsUpdateAndSave()
        {
            // Arrange
            var order = new Order { OrderId = 1, UserId = 1, Product = "Laptop", Quantity = 3, Price = 999.99m };
            _orderDbSetMock.Setup(m => m.Update(order));
            _dbContextMock.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _orderRepository.UpdateAsync(order);

            // Assert
            _orderDbSetMock.Verify(m => m.Update(order), Times.Once());
            _dbContextMock.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }

        [Test]
        public async Task DeleteAsync_ExistingId_CallsRemoveAndSave()
        {
            // Arrange
            var order = new Order { OrderId = 1, UserId = 1, Product = "Laptop", Quantity = 2, Price = 999.99m };
            _orderDbSetMock.Setup(m => m.FindAsync(1)).ReturnsAsync(order);
            _orderDbSetMock.Setup(m => m.Remove(order));
            _dbContextMock.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _orderRepository.DeleteAsync(1);

            // Assert
            _orderDbSetMock.Verify(m => m.FindAsync(1), Times.Once());
            _orderDbSetMock.Verify(m => m.Remove(order), Times.Once());
            _dbContextMock.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }

        [Test]
        public async Task DeleteAsync_NonExistentId_DoesNotCallRemove()
        {
            // Arrange
            _orderDbSetMock.Setup(m => m.FindAsync(999)).ReturnsAsync((Order)null);

            // Act
            await _orderRepository.DeleteAsync(999);

            // Assert
            _orderDbSetMock.Verify(m => m.FindAsync(999), Times.Once());
            _orderDbSetMock.Verify(m => m.Remove(It.IsAny<Order>()), Times.Never());
            _dbContextMock.Verify(m => m.SaveChangesAsync(default), Times.Never());
        }

        [Test]
        [TestCase("Laptop")]
        public async Task GetByIdAsync_ValidProduct_ReturnsOrder(string product)
        {
            // Arrange
            var order = new Order { OrderId = 1, UserId = 1, Product = product, Quantity = 2, Price = 999.99m };
            _orderDbSetMock.Setup(m => m.FindAsync(1)).ReturnsAsync(order);

            // Act
            var result = await _orderRepository.GetByIdAsync(1);

            // Assert
            Assert.That(result.Product, Is.EqualTo(product));
        }
    }
}