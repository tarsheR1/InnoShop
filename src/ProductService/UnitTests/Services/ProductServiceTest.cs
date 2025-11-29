
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using ProductService.Application.Services;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;

namespace ProductService.UnitTests.Services
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Application.Services.ProductService _productService;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productService = new Application.Services.ProductService(_unitOfWorkMock.Object);
            _fixture = new Fixture();

            // Настройка AutoFixture для избежания циклических ссылок
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        public async Task GetByIdAsync_WithValidId_ReturnsProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var expectedProduct = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .Without(p => p.Category) // Исключаем навигационное свойство
                .Create();

            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(productId))
                .ReturnsAsync(expectedProduct);

            var query = new GetProductByIdQuery(productId);

            // Act
            var result = await _productService.GetByIdAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedProduct);
            _unitOfWorkMock.Verify(u => u.Products.GetByIdAsync(productId), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(productId))
                .ReturnsAsync((Product)null);

            var query = new GetProductByIdQuery(productId);

            // Act
            var result = await _productService.GetByIdAsync(query);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task CreateProductAsync_WithValidCommand_CreatesProduct()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                .Without(c => c.Products)
                .Create();

            var command = _fixture.Build<CreateProductCommand>()
                .With(c => c.CategoryId, category.Id)
                .Create();

            _unitOfWorkMock.Setup(u => u.Categories.GetByIdAsync(command.CategoryId))
                .ReturnsAsync(category);
            _unitOfWorkMock.Setup(u => u.Products.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _productService.CreateProductAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(command.Name);
            result.Price.Should().Be(command.Price);
            result.CategoryId.Should().Be(command.CategoryId);
            result.CreatedByUserId.Should().Be(command.UserId);

            _unitOfWorkMock.Verify(u => u.Categories.GetByIdAsync(command.CategoryId), Times.Once);
            _unitOfWorkMock.Verify(u => u.Products.AddAsync(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void CreateProductAsync_WithInvalidCategory_ThrowsException()
        {
            // Arrange
            var command = _fixture.Create<CreateProductCommand>();
            _unitOfWorkMock.Setup(u => u.Categories.GetByIdAsync(command.CategoryId))
                .ReturnsAsync((Category)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() =>
                _productService.CreateProductAsync(command));
        }

        [Test]
        public void UpdateProductAsync_UserNotOwner_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var ownerUserId = "user1";
            var currentUserId = "user2";

            var existingProduct = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.CreatedByUserId, ownerUserId)
                .Without(p => p.Category)
                .Create();

            var command = _fixture.Build<UpdateProductCommand>()
                .With(c => c.ProductId, productId)
                .Create();

            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _productService.UpdateProductAsync(command, currentUserId));
        }

        [Test]
        [TestCase("user1", "user1", true)]   // Владелец - успешное удаление
        [TestCase("user1", "user2", false)]  // Не владелец - исключение
        public async Task DeleteProductAsync_WithDifferentUsers_ReturnsExpectedResult(
      string productOwnerId, string currentUserId, bool expectedResult)
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.CreatedByUserId, productOwnerId)
                .Without(p => p.Category)
                .Create();

            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(productId))
                .ReturnsAsync(product);

            // Настраиваем моки ТОЛЬКО если ожидается успешное удаление
            if (expectedResult)
            {
                _unitOfWorkMock.Setup(u => u.Products.DeleteAsync(It.IsAny<Product>()))
                    .Returns(Task.CompletedTask);
                _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                    .ReturnsAsync(1);
            }

            var command = new DeleteProductCommand(productId, currentUserId);

            // Act & Assert
            if (expectedResult)
            {
                // Ожидаем успешное выполнение
                var result = await _productService.DeleteProductAsync(command);
                result.Should().BeTrue();

                _unitOfWorkMock.Verify(u => u.Products.DeleteAsync(It.IsAny<Product>()), Times.Once);
                _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            }
            else
            {
                // Ожидаем исключение
                await _productService.Invoking(s => s.DeleteProductAsync(command))
                    .Should().ThrowAsync<UnauthorizedAccessException>()
                    .WithMessage("You can only delete your own products");

                _unitOfWorkMock.Verify(u => u.Products.DeleteAsync(It.IsAny<Product>()), Times.Never);
                _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
            }
        }
    }
}

