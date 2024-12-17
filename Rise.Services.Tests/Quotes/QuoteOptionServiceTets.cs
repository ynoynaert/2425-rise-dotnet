using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Rise.Domain.Exceptions;
using Rise.Persistence;
using Rise.Services.Customers;
using Rise.Services.Documents;
using Rise.Services.Files;
using Rise.Services.Machineries;
using Rise.Services.Quotes;
using Rise.Services.Translations;
using Rise.Shared.Quotes;
using Rise.Shared.Users;

namespace Rise.Services.Tests.Quotes;

public class QuoteOptionServiceTets : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Seeder _seeder;
    private readonly IQuoteOptionService _quoteOptionService;
    private readonly QuoteService _quoteService;
    private readonly TranslationService _translationService;
    private readonly CustomerService _customerService;
    private readonly MachineryService _machineryService;
    private readonly DocumentService _docuementService;
    private readonly IStorageService _storageService;
    private readonly IConfiguration _configuration;
    private readonly Mock<IUserService> _userService;


    public QuoteOptionServiceTets()
    {
        _context = TestApplicationDbContextFactory.CreateDbContext();
        _context.Database.Migrate();

        _seeder = new Seeder(_context);
        _seeder.Seed();

        _configuration = new ConfigurationBuilder()
         .AddInMemoryCollection(new Dictionary<string, string?>
         {
            { "DeepL:APIKey", "fake-test-key" },
            { "ConnectionStrings:Storage", "test" }
         })
         .Build();

        _storageService = new BlobStorageService(_configuration);
        _translationService = new TranslationService(_context, _configuration);
        _customerService = new CustomerService(_context, _translationService);
        _machineryService = new MachineryService(_storageService, _context);
        _userService = new Mock<IUserService>();
        _docuementService = new DocumentService(_userService.Object);
        _quoteService = new QuoteService(_context, _translationService, _customerService, _machineryService, _userService.Object, _docuementService);
        _quoteOptionService = new QuoteOptionService(_context, _quoteService);
    }

    [Fact]
    public async Task CreateQuoteOptionAsync_ShouldCreateNewQuoteOption()
    {
        // Arrange
        var quote = _context.Quotes.First();
        var machineryOption = _context.MachineryOptions.First();

        var createDto = new QuoteOptionDto.Create
        {
            QuoteId = quote.Id,
            MachineryOptionId = machineryOption.Id
        };

        // Act
        var result = await _quoteOptionService.CreateQuoteOptionAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(quote.Id, result.Quote.Id);
        Assert.Equal(machineryOption.Id, result.MachineryOption.Id);

        var createdOption = _context.QuoteOptions.SingleOrDefault(qo => qo.Id == result.Id);
        Assert.NotNull(createdOption);
    }

    [Fact]
    public async Task DeleteQuoteOptionAsync_ShouldThrowException_IfNotFound()
    {
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _quoteOptionService.DeleteQuoteOptionAsync(-1));
    }

    [Fact]
    public async Task GetQuoteOptionAsync_ShouldThrowException_IfNotFound()
    {
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _quoteOptionService.GetQuoteOptionAsync(-1));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
