using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Rise.Domain.Exceptions;
using Rise.Persistence;
using Rise.Services.Files;
using Rise.Services.Machineries;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;

namespace Rise.Services.Tests.Machineries;

public class MachineryServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Seeder _seeder;
    private readonly MachineryService _machineryService;
    private readonly IStorageService _storageService;
    private readonly IConfiguration _configuration;

    public OptionQueryObject query = new OptionQueryObject();

    public MachineryServiceTest()
    {
        _context = TestApplicationDbContextFactory.CreateDbContext();
        _context.Database.Migrate();

        _seeder = new Seeder(_context);
        _seeder.Seed();

        _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:Storage", "fake-test-key" }
        })
        .Build();

        _storageService = new BlobStorageService(_configuration);
        _machineryService = new MachineryService(_storageService, _context);
    }

    [Fact]
    public async Task GetMachineriesAsync_ShouldReturnAllMachineries()
    {
        // Arrange
        var mqo = new MachineryQueryObject();
        var expectedMachineries = _context.Machineries.Where(x => !x.IsDeleted).ToList();
        var expectedCount = expectedMachineries.Skip((mqo.PageNumber - 1) * mqo.PageSize).Take(mqo.PageSize).Count();

        // Act
        var machineries = await _machineryService.GetMachineriesAsync(mqo);

        // Assert
        Assert.Equal(expectedCount, machineries.Count());
    }

    [Fact]
    public async Task GetMachineriesAsync_ShouldFilterBySearchQuery()
    {
        // Arrange
        var mqo = new MachineryQueryObject { Search = "Cat" };
        var expectedMachineries = _context.Machineries
            .Where(x => !x.IsDeleted && x.Name.Contains("Cat"))
            .ToList();

        // Act
        var machineries = await _machineryService.GetMachineriesAsync(mqo);

        // Assert
        Assert.All(machineries, m => Assert.Contains("Cat", m.Name));
    }

    [Fact]
    public async Task GetMachineriesAsync_ShouldFilterByType()
    {
        // Arrange
        var typeId = _context.MachineryTypes.First().Id;
        var mqo = new MachineryQueryObject { TypeIds = typeId.ToString() };

        var expectedMachineries = _context.Machineries
            .Where(x => !x.IsDeleted && x.Type.Id == typeId)
            .ToList();

        // Act
        var machineries = await _machineryService.GetMachineriesAsync(mqo);

        // Assert
        Assert.All(machineries, m => Assert.Equal(typeId, m.Type.Id));
    }

    [Fact]
    public async Task GetMachineriesAsync_ShouldSortByNameAscending()
    {
        // Arrange
        var mqo = new MachineryQueryObject { SortBy = "Name", IsDescending = false };
        var expectedMachineries = _context.Machineries
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToList();

        // Act
        var machineries = await _machineryService.GetMachineriesAsync(mqo);

        // Assert
        Assert.True(machineries.Zip(expectedMachineries, (a, b) => a.Name == b.Name).All(x => x));
    }

    [Fact]
    public async Task GetMachineryAsync_ShouldReturnMachinery()
    {
        // Arrange
        var id = _context.Machineries.First().Id;

        // Act
        var machinery = await _machineryService.GetMachineryAsync(id);

        // Assert
        Assert.NotNull(machinery);
    }

    [Fact]
    public async Task GetMachineryAsync_ShouldThrowException_MachineryDoesNotExist()
    {
        // Arrange
        var id = (await _context.Machineries.MaxAsync(x => (int?)x.Id) ?? 0) + 1;

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryService.GetMachineryAsync(id));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task CreateMachineryAsync_ShouldCreateMachinery()
    {
        // Arrange
        var machineryDto = new MachineryDto.Create
        {
            Name = "Test",
            SerialNumber = "1234",
            TypeId = _context.MachineryTypes.First().Id,
            Description = "Test Description",
            BrochureText = "Test Brochure"
        };

        // Act
        var machinery = await _machineryService.CreateMachineryAsync(machineryDto);

        // Assert
        Assert.NotNull(machinery);
    }

    [Fact]
    public async Task CreateMachineryAsync_ShouldThrowException_SerialNumberAlreadyUsed()
    {
        // Arrange
        var machineryDto = new MachineryDto.Create
        {
            Name = "Test",
            SerialNumber = _context.Machineries.First().SerialNumber,
            TypeId = _context.MachineryTypes.First().Id,
            Description = "Test Description",
            BrochureText = "Test Brochure"
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryService.CreateMachineryAsync(machineryDto));

        // Assert
        Assert.IsType<EntityAlreadyExistsException>(exception);
    }

    [Fact]
    public async Task CreateMachineryAsync_ShouldThrowException_TypeDoesNotExist()
    {
        // Arrange
        var machineryDto = new MachineryDto.Create
        {
            Name = "Test",
            SerialNumber = "1234",
            TypeId = (await _context.MachineryTypes.MaxAsync(x => (int?)x.Id) ?? 0) + 1,
            Description = "Test Description",
            BrochureText = "Test Brochure"
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryService.CreateMachineryAsync(machineryDto));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task UpdateMachineryAsync_ShouldThrowException_MachineryDoesNotExist()
    {
        // Arrange
        var id = (await _context.Machineries.MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        var machineryDto = new MachineryDto.Update
        {
            Id = id,
            Name = "Test",
            SerialNumber = "1234",
            TypeId = _context.MachineryTypes.First().Id,
            Description = "Test Description",
            BrochureText = "Test Brochure"
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryService.UpdateMachineryAsync(id, machineryDto));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task UpdateMachineryAsync_ShouldThrowException_EntityAlreadyExists()
    {
        // Arrange
        var id = 2;
        var machineryDto = new MachineryDto.Update
        {
            Id = id,
            Name = "Test",
            SerialNumber = "1234",
            TypeId = (await _context.MachineryTypes.MaxAsync(x => (int?)x.Id) ?? 0) + 1,
            Description = "Test Description",
            BrochureText = "Test Brochure"
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryService.UpdateMachineryAsync(id, machineryDto));

        // Assert
        Assert.IsType<EntityAlreadyExistsException>(exception);
    }

    [Fact]
    public async Task DeleteMachineryAsync_ShouldThrowException_MachineryDoesNotExist()
    {
        // Arrange
        var id = (await _context.Machineries.MaxAsync(x => (int?)x.Id) ?? 0) + 1;

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryService.DeleteMachineryAsync(id));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task GetMachineryAsyncWithCategories_ShouldReturnMachineriesWithCategories()
    {
        // Arrange
        var id = _context.Machineries.First().Id;

        // Act
        var machinery = await _machineryService.GetMachineryAsyncWithCategories(id, query);

        // Assert
        Assert.NotNull(machinery);
    }

    [Fact]
    public async Task GetMachineryAsyncWithCategories_ShouldThrowException_MachineryDoesNotExist()
    {
        // Arrange
        var id = (await _context.Machineries.MaxAsync(x => (int?)x.Id) ?? 0) + 1;

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryService.GetMachineryAsyncWithCategories(id, query));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task GetMachineryAsyncWithCategories_ShouldFilterBySearchQuery()
    {
        // Arrange
        var id = _context.Machineries.First().MachineryOptions.First();

        // Set the search query to match one of the options or categories
        query.Search = id.ToString();

        // Act
        var result = await _machineryService.GetMachineryAsyncWithCategories(1, query);

        // Assert
        Assert.NotNull(result);
    }


    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
