using AutoFixture;
using Moq;
using NUnit.Framework;
using ProductService.Application.Services;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;
using FluentAssertions;

[TestFixture]
public class CategoryServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private CategoryService _categoryService;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryService = new CategoryService(_unitOfWorkMock.Object);
        _fixture = new Fixture();

        // ИСПРАВЛЕНИЕ: Убираем циклические ссылки
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Test]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = _fixture.Build<Category>()
            .Without(c => c.Products) // ← ИСКЛЮЧАЕМ навигационное свойство
            .CreateMany(3)
            .ToList();

        _unitOfWorkMock.Setup(u => u.Categories.GetAllAsync())
            .ReturnsAsync(categories);

        var query = new GetAllCategoriesQuery();

        // Act
        var result = await _categoryService.GetAllCategoriesAsync(query);

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(categories);
    }

    [Test]
    public async Task DeleteCategoryAsync_WhenCategoryHasProducts_ThrowsException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = _fixture.Build<Category>()
            .With(c => c.Id, categoryId)
            .Without(c => c.Products) // ← ИСКЛЮЧАЕМ навигационное свойство
            .Create();

        var searchResult = new ProductSearchResult(
            _fixture.Build<Product>()
                .Without(p => p.Category) // ← ИСКЛЮЧАЕМ навигационное свойство
                .CreateMany(2)
                .ToList(),
            2);

        _unitOfWorkMock.Setup(u => u.Categories.GetByIdAsync(categoryId))
            .ReturnsAsync(category);
        _unitOfWorkMock.Setup(u => u.Products.SearchAsync(It.IsAny<ProductSearchCriteria>()))
            .ReturnsAsync(searchResult);

        // Act & Assert
        await _categoryService.Invoking(s => s.DeleteCategoryAsync(categoryId))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot delete category with existing products");
    }

    [Test]
    public async Task CreateCategoryAsync_WithValidCommand_CreatesCategory()
    {
        // Arrange
        var command = _fixture.Create<CreateCategoryCommand>();
        _unitOfWorkMock.Setup(u => u.Categories.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _categoryService.CreateCategoryAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
    }
}