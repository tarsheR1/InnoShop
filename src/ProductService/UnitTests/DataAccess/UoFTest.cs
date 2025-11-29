using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;
using ProductService.Infrastructure.DataAccess;
using ProductService.Infrastructure.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.DataAccess
{
    [TestFixture]
    public class UnitOfWorkTests
    {
        private IFixture _fixture;
        private Mock<ProductServiceDbContext> _mockContext;
        private UnitOfWork _unitOfWork;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _mockContext = new Mock<ProductServiceDbContext>(new DbContextOptionsBuilder<ProductServiceDbContext>().Options);

            // Для UnitOfWork тестов нам не нужно мокать DbSet'ы, так как мы тестируем только
            // создание репозиториев и вызовы SaveChanges/Dispose
            _unitOfWork = new UnitOfWork(_mockContext.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork?.Dispose();
        }

        [Test]
        public void Constructor_ShouldInitializeRepositories()
        {
            // Assert
            _unitOfWork.Products.Should().NotBeNull();
            _unitOfWork.Products.Should().BeOfType<ProductRepository>();

            _unitOfWork.Categories.Should().NotBeNull();
            _unitOfWork.Categories.Should().BeOfType<CategoryRepository>();
        }

        [Test]
        public void Products_Property_ShouldReturnProductRepository()
        {
            // Act & Assert
            _unitOfWork.Products.Should().NotBeNull();
            _unitOfWork.Products.Should().BeAssignableTo<IProductRepository>();
        }

        [Test]
        public void Categories_Property_ShouldReturnCategoryRepository()
        {
            // Act & Assert
            _unitOfWork.Categories.Should().NotBeNull();
            _unitOfWork.Categories.Should().BeAssignableTo<ICategoryRepository>();
        }

        [Test]
        public async Task SaveChangesAsync_ShouldCallDbContextSaveChanges()
        {
            // Arrange
            var expectedChanges = 3;
            _mockContext.Setup(c => c.SaveChangesAsync(default))
                       .ReturnsAsync(expectedChanges);

            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().Be(expectedChanges);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task SaveChangesAsync_WhenDbContextThrowsException_ShouldPropagateException()
        {
            // Arrange
            var expectedException = new DbUpdateException("Test exception");
            _mockContext.Setup(c => c.SaveChangesAsync(default))
                       .ThrowsAsync(expectedException);

            // Act
            Func<Task> act = async () => await _unitOfWork.SaveChangesAsync();

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>().WithMessage("Test exception");
        }

        [Test]
        public void Dispose_ShouldDisposeDbContext()
        {
            // Act
            _unitOfWork.Dispose();

            // Assert
            _mockContext.Verify(c => c.Dispose(), Times.Once);
        }

        [Test]
        public void Dispose_WhenDbContextIsNull_ShouldNotThrow()
        {
            // Arrange
            var unitOfWorkWithNullContext = new UnitOfWork(null);

            // Act
            Action act = () => unitOfWorkWithNullContext.Dispose();

            // Assert
            act.Should().NotThrow();

            // Cleanup
            unitOfWorkWithNullContext.Dispose();
        }

        [Test]
        public void Repositories_ShouldUseSameDbContextInstance()
        {
            // Arrange
            var productRepository = _unitOfWork.Products as ProductRepository;
            var categoryRepository = _unitOfWork.Categories as CategoryRepository;

            // Act & Assert
            productRepository.Should().NotBeNull();
            categoryRepository.Should().NotBeNull();

            // Проверяем, что репозитории используют один и тот же контекст
            var productRepoContext = GetPrivateField<ProductRepository, ProductServiceDbContext>(productRepository, "_context");
            var categoryRepoContext = GetPrivateField<CategoryRepository, ProductServiceDbContext>(categoryRepository, "_context");

            productRepoContext.Should().BeSameAs(_mockContext.Object);
            categoryRepoContext.Should().BeSameAs(_mockContext.Object);
        }

        // Вспомогательный метод для доступа к приватным полям через reflection
        private static TField GetPrivateField<TInstance, TField>(TInstance instance, string fieldName)
        {
            var fieldInfo = typeof(TInstance).GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (TField)fieldInfo?.GetValue(instance);
        }
    }

    // Отдельные тесты для репозиториев с использованием InMemory database
    [TestFixture]
    public class UnitOfWorkIntegrationTests
    {
        private UnitOfWork _unitOfWork;
        private ProductServiceDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ProductServiceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ProductServiceDbContext(options);
            _unitOfWork = new UnitOfWork(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork?.Dispose();
            _context?.Dispose();
        }

        [Test]
        public async Task CompleteWorkflow_WithRealRepositories_ShouldWorkCorrectly()
        {
            // Arrange
            var category = new Category { Id = Guid.NewGuid(), Name = "Test Category" };
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                CategoryId = category.Id,
                Price = 100,
                IsAvailable = true
            };

            // Act
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.Products.AddAsync(product);
            var savedChanges = await _unitOfWork.SaveChangesAsync();

            // Assert
            savedChanges.Should().Be(2);

            var savedProduct = await _unitOfWork.Products.GetByIdAsync(product.Id);
            savedProduct.Should().NotBeNull();
            savedProduct.Name.Should().Be("Test Product");
        }

        [Test]
        public async Task ProductsRepository_CanSearchProducts()
        {
            // Arrange
            var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                CategoryId = category.Id,
                Price = 1000,
                IsAvailable = true
            };
            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Mouse",
                CategoryId = category.Id,
                Price = 50,
                IsAvailable = true
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.Products.AddAsync(product1);
            await _unitOfWork.Products.AddAsync(product2);
            await _unitOfWork.SaveChangesAsync();

            var criteria = new ProductSearchCriteria { Name = "Laptop" };

            // Act
            var result = await _unitOfWork.Products.SearchAsync(criteria);

            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(1);
            result.Products.Should().ContainSingle(p => p.Name == "Laptop");
        }
    }

    // Тесты для отдельных операций репозиториев с моками
    [TestFixture]
    public class RepositoryOperationsTests
    {
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

    }


}
        