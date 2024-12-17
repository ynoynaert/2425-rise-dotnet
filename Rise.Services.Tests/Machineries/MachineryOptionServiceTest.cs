using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Persistence;
using Rise.Services.Machineries;
using Rise.Shared.Machineries;

namespace Rise.Services.Tests.Machineries;

public class MachineryOptionServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Seeder _seeder;
    private readonly MachineryOptionService _machineryOptionService;
    public MachineryOptionServiceTest()
    {
        _context = TestApplicationDbContextFactory.CreateDbContext();
        _context.Database.Migrate();

        _seeder = new Seeder(_context);
        _seeder.Seed();
        _machineryOptionService = new MachineryOptionService(_context);
    }

    [Fact]
    public async Task GetMachineryOptionsAsync_ShouldReturnAllMachineryOptions()
    {
        // Arrange
        var expectedCount = _context.MachineryOptions.Count(x => !x.IsDeleted);

        // Act
        var machineryOptions = await _machineryOptionService.GetMachineryOptionsAsync();

        // Assert
        Assert.Equal(expectedCount, machineryOptions.Count());
    }

    [Fact]
    public async Task GetMachineryOptionAsync_ShouldReturnMachineryOption()
    {
        // Arrange
        var id = await _context.MachineryOptions.Where(x => !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsync();

        // Act
        var machineryOption = await _machineryOptionService.GetMachineryOptionAsync(id);

        // Assert
        Assert.NotNull(machineryOption);
    }

    [Fact]
    public async Task GetMachineryOptionAsync_ShouldThrowException_MachineryOptionNotFound()
    {
        // Arrange
        var id = (await _context.MachineryOptions.MaxAsync(x => (int?)x.Id) ?? 0) + 1;

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryOptionService.GetMachineryOptionAsync(id));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task CreateMachineryOptionAsync_ShouldCreateMachineryOption()
    {
        // Arrange
        var machineryOptionDto = new MachineryOptionDto.Create
        {
            MachineryId = 1,
            OptionId = 1,
            Price = 100
        };

        // Act
        var machineryOption = await _machineryOptionService.CreateMachineryOptionAsync(machineryOptionDto);

        // Assert
        Assert.NotNull(machineryOption);
    }

    [Fact]
    public async Task CreateMachineryOptionAsync_ShouldThrowException_MachineryNotFound()
    {
        // Arrange
        var machineryOptionDto = new MachineryOptionDto.Create
        {
            MachineryId = 100,
            OptionId = 1,
            Price = 100
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryOptionService.CreateMachineryOptionAsync(machineryOptionDto));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task CreateMachineryOptionAsync_ShouldThrowException_OptionNotFound()
    {
        // Arrange
        var machineryOptionDto = new MachineryOptionDto.Create
        {
            MachineryId = 1,
            OptionId = 100,
            Price = 100
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryOptionService.CreateMachineryOptionAsync(machineryOptionDto));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task UpdateMachineryOptionAsync_ShouldUpdateMachineryOption()
    {
        // Arrange
        var cat = await _context.Categories.Where(x => !x.IsDeleted).FirstOrDefaultAsync();
        var machineryOptionDto = new MachineryOptionDto.Update
        {
            Id = cat!.Id,
            MachineryId = 1,
            OptionId = 1,
            Price = 200
        };

        // Act
        var machineryOption = await _machineryOptionService.UpdateMachineryOptionAsync(cat!.Id, machineryOptionDto);

        // Assert
        Assert.Equal(machineryOptionDto.Price, machineryOption.Price);
    }

    [Fact]
    public async Task UpdateMachineryOptionAsync_ShouldThrowException_MachineryOptionNotFound()
    {
        // Arrange
        var id = (await _context.MachineryOptions.MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        var machineryOptionDto = new MachineryOptionDto.Update
        {
            Id = id,
            MachineryId = 1,
            OptionId = 1,
            Price = 200
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryOptionService.UpdateMachineryOptionAsync(id, machineryOptionDto));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task UpdateMachineryOptionAsync_ShouldThrowException_MachineryNotFound()
    {
        // Arrange
        var cat = await _context.Categories.Where(x => !x.IsDeleted).FirstOrDefaultAsync();
        var machineryOptionDto = new MachineryOptionDto.Update
        {
            Id = cat!.Id,
            MachineryId = 100,
            OptionId = 1,
            Price = 200
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryOptionService.UpdateMachineryOptionAsync(cat!.Id, machineryOptionDto));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task UpdateMachineryOptionAsync_ShouldThrowException_OptionNotFound()
    {
        // Arrange
        var cat = await _context.Categories.Where(x => !x.IsDeleted).FirstOrDefaultAsync();
        var machineryOptionDto = new MachineryOptionDto.Update
        {
            Id = cat!.Id,
            MachineryId = 1,
            OptionId = 100,
            Price = 200
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryOptionService.UpdateMachineryOptionAsync(cat!.Id, machineryOptionDto));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task DeleteMachineryOptionAsync_ShouldDeleteMachineryOption()
    {
        // Arrange
        var mo = await _context.MachineryOptions.Where(x => !x.IsDeleted).FirstOrDefaultAsync();

        // Act
        await _machineryOptionService.DeleteMachineryOptionAsync(mo!.Id);

        // Assert
        var machineryOption = await _context.MachineryOptions.Where(x => !x.IsDeleted).SingleOrDefaultAsync(x => x.Id == mo.Id);
        Assert.Null(machineryOption);
    }

    [Fact]
    public async Task DeleteMachineryOptionAsync_ShouldThrowException_MachineryOptionNotFound()
    {
        // Arrange
        var id = (await _context.MachineryOptions.MaxAsync(x => (int?)x.Id) ?? 0) + 1;

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryOptionService.DeleteMachineryOptionAsync(id));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
