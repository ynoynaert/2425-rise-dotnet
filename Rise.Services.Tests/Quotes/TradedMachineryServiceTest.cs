using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rise.Domain.Exceptions;
using Rise.Persistence;
using Rise.Services.Files;
using Rise.Services.Quotes;
using Rise.Shared.Quotes;

namespace Rise.Services.Tests.Quotes;

public class TradedMachineryServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IStorageService _storageService;
    private readonly TradedMachineryService _tradedMachineryService;
    private readonly IConfiguration _configuration;
    public TradedMachineryServiceTest()
    {
        _context = TestApplicationDbContextFactory.CreateDbContext();
        _context.Database.Migrate();
        Seeder seeder = new Seeder(_context);
        seeder.Seed();

        _configuration = new ConfigurationBuilder()
       .AddInMemoryCollection(new Dictionary<string, string?>
       {
            { "ConnectionStrings:Storage", "fake-test-key" }
       })
       .Build();

        _storageService = new BlobStorageService(_configuration);
        _tradedMachineryService = new TradedMachineryService(_storageService, _context);
    }

    [Fact]
    public async Task CreateTradedMachineryAsync_ShouldCreateTradedMachinery_WhenValidInput()
    {
        // Arrange
        var existingQuote = _context.Quotes.Include(q => q.TradedMachineries).Where(x=>x.IsApproved).First();

        var tradedMachineryDto = new TradedMachineryDto.Create
        {
            QuoteNumber = existingQuote.QuoteNumber,
            Name = "Test Machine",
            TypeId = existingQuote.Id,
            SerialNumber = "SN123456",
            Description = "Test Description",
            EstimatedValue = 1000m,
            Year = 2020,
        };

        // Act
        var result = await _tradedMachineryService.CreateTradedMachineryAsync(tradedMachineryDto);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateTradedMachineryAsync_ShouldThrowException_WhenQuoteNotFound()
    {
        // Arrange
        var tradedMachineryDto = new TradedMachineryDto.Create
        {
            QuoteNumber = "NON_EXISTENT_QUOTE",
            Name = "Test Machine",
            TypeId = _context.MachineryTypes.First().Id,
            SerialNumber = "SN123456",
            Description = "Test Description",
            EstimatedValue = 1000m,
            Year = 2020,
            ImageContentType = { "image/png" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _tradedMachineryService.CreateTradedMachineryAsync(tradedMachineryDto));
    }

    [Fact]
    public async Task CreateTradedMachineryAsync_ShouldThrowException_WhenQuoteNotApproved()
    {
        // Arrange
        var existingQuote = _context.Quotes.Where(x=>!x.IsApproved).First();

        var tradedMachineryDto = new TradedMachineryDto.Create
        {
            QuoteNumber = existingQuote.QuoteNumber,
            Name = "Test Machine",
            TypeId = _context.MachineryTypes.First().Id,
            SerialNumber = "SN123456",
            Description = "Test Description",
            EstimatedValue = 1000m,
            Year = 2020,
            ImageContentType = { "image/png" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tradedMachineryService.CreateTradedMachineryAsync(tradedMachineryDto));
    }

    [Fact]
    public async Task CreateTradedMachineryAsync_ShouldThrowException_WhenMachineryTypeNotFound()
    {
        // Arrange
        var existingQuote = _context.Quotes.First();
        existingQuote.IsApproved = true;
        _context.SaveChanges();

        var tradedMachineryDto = new TradedMachineryDto.Create
        {
            QuoteNumber = existingQuote.QuoteNumber,
            Name = "Test Machine",
            TypeId = -1, 
            SerialNumber = "SN123456",
            Description = "Test Description",
            EstimatedValue = 1000m,
            Year = 2020,
            ImageContentType = { "image/png" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _tradedMachineryService.CreateTradedMachineryAsync(tradedMachineryDto));
    }

    [Fact]
    public async Task CreateTradedMachineryAsync_ShouldThrowException_WhenEstimatedValueTooHigh()
    {
        // Arrange
        var existingQuote = _context.Quotes.Include(q => q.TradedMachineries).First();
        existingQuote.IsApproved = true;
        _context.SaveChanges();

        var tradedMachineryDto = new TradedMachineryDto.Create
        {
            QuoteNumber = existingQuote.QuoteNumber,
            Name = "Test Machine",
            TypeId = _context.MachineryTypes.First().Id,
            SerialNumber = "SN123456",
            Description = "Test Description",
            EstimatedValue = existingQuote.TotalWithVat + 1,
            Year = 2020,
            ImageContentType ={ "image/png" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tradedMachineryService.CreateTradedMachineryAsync(tradedMachineryDto));
    }


    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
