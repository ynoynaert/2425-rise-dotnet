using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Persistence;
using Rise.Services.Machineries;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;

namespace Rise.Services.Tests.Machineries;

public class CategoryServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Seeder _seeder;
    private readonly CategoryService _categoryService;
    public CategoryServiceTest()
    {
        _context = TestApplicationDbContextFactory.CreateDbContext();

        _context.Database.Migrate();

        _seeder = new Seeder(_context);
        _seeder.Seed();
        _categoryService = new CategoryService(_context);
    }

    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnAllCategories()
    {
        // Arrange
        var queryObject = new CategoryQueryObject();
        var expectedCount = _context.Categories.Count(x => !x.IsDeleted);

        // Act
        var categories = await _categoryService.GetCategoriesAsync(queryObject);

        // Assert
        Assert.Equal(expectedCount, categories.Count());
    }

    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnCategoriesWithSearch()
    {
        // Arrange
        var queryObject = new CategoryQueryObject { Search = "Comfort" };

        // Act
        var categories = await _categoryService.GetCategoriesAsync(queryObject);

        // Assert
        Assert.Single(categories);
    }

    [Fact]
    public async Task GetCategoryAsync_ShouldReturnCategory()
    {
        // Arrange
        var cat = await _context.Categories.Where(x => !x.IsDeleted).FirstOrDefaultAsync();

        // Act
        var category = await _categoryService.GetCategoryAsync(cat!.Id);

        // Assert
        Assert.Equal(cat.Code, category.Code);
    }

    [Fact]
    public async Task GetCategoryAsync_ShouldThrowException_CategoryDoesNotExist()
    {
        // Arrange
        var id = (await _context.Categories.MaxAsync(x => (int?)x.Id) ?? 0) + 1;

        // Act
        var exception = await Record.ExceptionAsync(() => _categoryService.GetCategoryAsync(id));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldCreateCategory()
    {
        // Arrange
        var categoryDto = new CategoryDto.Create
        {
            Name = "Test",
            Code = "TST"
        };

        // Act
        var category = await _categoryService.CreateCategoryAsync(categoryDto);

        // Assert
        Assert.NotNull(category);
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldThrowException_CodeAlreadyUsed()
    {
        // Arrange
        var categoryDto = new CategoryDto.Create
        {
            Name = "Test",
            Code = "1200"
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _categoryService.CreateCategoryAsync(categoryDto));

        // Assert
        Assert.IsType<EntityAlreadyExistsException>(exception);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldUpdateCategory()
    {
        // Arrange
        var cat = await _context.Categories.Where(x => !x.IsDeleted).FirstOrDefaultAsync();
        var categoryDto = new CategoryDto.Update
        {
            Id = cat!.Id,
            Name = "Test",
            Code = "TST"
        };

        // Act
        var category = await _categoryService.UpdateCategoryAsync(cat!.Id, categoryDto);

        // Assert
        Assert.NotNull(category);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldThrowException_CategoryDoesNotExist()
    {
        // Arrange
        var id = (await _context.Categories.MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        var categoryDto = new CategoryDto.Update
        {
            Id = id,
            Name = "Test",
            Code = "TST"
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _categoryService.UpdateCategoryAsync(id, categoryDto));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldThrowException_CodeAlreadyUsed()
    {
        // Arrange
        var id = 2;

        var categoryDto = new CategoryDto.Update
        {
            Id = id,
            Name = "Test",
            Code = "1200"
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _categoryService.UpdateCategoryAsync(id, categoryDto));

        // Assert
        Assert.IsType<EntityAlreadyExistsException>(exception);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldDeleteCategory()
    {
        // Arrange
        var cat = await _context.Categories.Where(x => !x.IsDeleted).FirstOrDefaultAsync();

        // Act
        await _categoryService.DeleteCategoryAsync(cat!.Id);

        // Assert
        var category = await _context.Categories.Where(x => !x.IsDeleted).SingleOrDefaultAsync(x => x.Id == cat!.Id);
        Assert.Null(category);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldThrowException_CategoryDoesNotExist()
    {
        // Arrange
        var id = (await _context.Categories.MaxAsync(x => (int?)x.Id) ?? 0) + 1;

        // Act
        var exception = await Record.ExceptionAsync(() => _categoryService.DeleteCategoryAsync(id));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
