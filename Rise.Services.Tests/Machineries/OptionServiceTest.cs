using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Persistence;
using Rise.Services.Machineries;
using Rise.Shared.Machineries;

namespace Rise.Services.Tests.Machineries;

public class OptionServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Seeder _seeder;
    private readonly OptionService _optionService;

    public OptionServiceTest()
    {
        _context = TestApplicationDbContextFactory.CreateDbContext();

        _context.Database.Migrate();

        _seeder = new Seeder(_context);
        _seeder.Seed();
        _optionService = new OptionService(_context);
    }

    [Fact]
    public async void getOptions_ShouldReturnOptions()
    {
        // Arrange
        var options = _context.Options.Where(x => !x.IsDeleted).ToList();

        // Act
        var result = await _optionService.GetOptionsAsync();

        // Assert
        Assert.Equal(options.Count, result.Count());
    }

    [Fact]
    public async void getOption_ShouldReturnOption()
    {
        // Arrange
        var option = _context.Options.FirstOrDefault();

        // Act
        var result = await _optionService.GetOptionAsync(option!.Id);

        // Assert
        Assert.Equal(option.Name, result.Name);
    }

    [Fact]
    public async void createOption_ShouldCreateOption()
    {
        // Arrange
        var option = new OptionDto.Create
        {
            Name = "Test Option",
            Code = "TO",
            CategoryId = 1
        };

        // Act
        var result = await _optionService.CreateOptionAsync(option);

        // Assert
        Assert.Equal(option.Name, result.Name);
    }

    [Fact]
    public async void createOption_ShouldThrowEntityAlreadyExistsException()
    {
        // Arrange
        var option = new OptionDto.Create
        {
            Name = "Test Option",
            Code = "TO",
            CategoryId = 1
        };

        // Act
        await _optionService.CreateOptionAsync(option);

        // Assert
        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _optionService.CreateOptionAsync(option));
    }

    [Fact]
    public async void createOption_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var option = new OptionDto.Create
        {
            Name = "Test Option",
            Code = "TO",
            CategoryId = 1
        };

        // Act
        await _optionService.CreateOptionAsync(option);

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _optionService.GetOptionAsync(0));
    }

    [Fact]
    public async Task UpdateOption_ShouldUpdateOption()
    {
        // Arrange
        var existingOption = _context.Options.First();
        var optionDto = new OptionDto.Update
        {
            Id = existingOption.Id,
            Name = "Updated Option",
            Code = "UO",
            CategoryId = existingOption.Category.Id
        };

        // Act
        var result = await _optionService.UpdateOptionAsync(existingOption.Id, optionDto);

        // Assert
        var updatedOption = _context.Options.First(x => x.Id == existingOption.Id);
        Assert.NotNull(updatedOption);
        Assert.Equal("Updated Option", updatedOption.Name);
        Assert.Equal("UO", updatedOption.Code);
    }


    [Fact]
    public async Task UpdateOption_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var nonExistentOptionId = 0;
        var optionDto = new OptionDto.Update
        {
            Id = nonExistentOptionId,
            Name = "Updated Option",
            Code = "UO",
            CategoryId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            async () => await _optionService.UpdateOptionAsync(nonExistentOptionId, optionDto)
        );
    }


    [Fact]
    public async void DeleteOption_ShouldDeleteOption()
    {
        // Arrange
        var option = _context.Options.FirstOrDefault();

        // Act
        await _optionService.DeleteOptionAsync(option!.Id);

        // Assert
        var result = await _context.Options.Where(x => !x.IsDeleted).SingleOrDefaultAsync(x => x.Id == option.Id);
        Assert.Null(result);
    }

    [Fact]
    public async void DeleteOption_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var option = _context.Options.FirstOrDefault();

        // Act
        await _optionService.DeleteOptionAsync(option!.Id);

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _optionService.DeleteOptionAsync(0));
    }


    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
