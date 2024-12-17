using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Rise.Domain.Exceptions;
using Rise.Domain.Quotes;
using Rise.Persistence;
using Rise.Services.Customers;
using Rise.Services.Documents;
using Rise.Services.Files;
using Rise.Services.Machineries;
using Rise.Services.Quotes;
using Rise.Services.Translations;
using Rise.Services.Users;
using Rise.Shared.Documents;
using Rise.Shared.Helpers;
using Rise.Shared.Locations;
using Rise.Shared.Quotes;
using Rise.Shared.Users;
using static Rise.Shared.Quotes.QuoteDto;

namespace Rise.Services.Tests.Quotes;

public class QuoteServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Seeder _seeder;
    private readonly QuoteService _quoteService;
    private readonly IConfiguration _configuration;
    private readonly IStorageService _storageService;
    private readonly TranslationService _translationService;
    private readonly CustomerService _customerService;
    private readonly MachineryService _machineryService;
    private readonly Mock<IUserService> _userService;
    private readonly DocumentService _docuementService;

    public QuoteServiceTest()
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
    }

    [Fact]
    public async void GetQuotesAsync_ShouldReturnAllQuotes()
    {
        // Arrange
        var query = new QuoteQueryObject { PageSize = 1 };

        // Act
        var result = await _quoteService.GetQuotesAsync(query);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async void GetQuotesAsync_WithSearch_ShouldReturnFilteredQuotes()
    {
        // Arrange
        var query = new QuoteQueryObject
        {
            PageSize = 5,
            Search = "Customer1" 
        };

        // Act
        var result = await _quoteService.GetQuotesAsync(query);

        // Assert
        Assert.All(result, quote => Assert.Contains("Customer1", quote.Customer.Name));
    }

    [Fact]
    public async void GetQuotesAsync_WithDateFilters_ShouldReturnQuotesWithinRange()
    {
        // Arrange
        var query = new QuoteQueryObject
        {
            PageSize = 5,
            After = new DateTime(2023, 1, 1),
            Before = new DateTime(2023, 12, 31)
        };

        // Act
        var result = await _quoteService.GetQuotesAsync(query);

        // Assert
        Assert.All(result, quote =>
            Assert.True(quote.Date > DateTime.Parse("2023-01-01") && quote.Date < DateTime.Parse("2023-12-31")));
    }

    [Fact]
    public async void GetQuotesAsync_WithSorting_ShouldReturnSortedQuotes()
    {
        // Arrange
        var query = new QuoteQueryObject
        {
            PageSize = 5,
            SortBy = "QuoteNumber",
            IsDescending = false
        };

        // Act
        var result = await _quoteService.GetQuotesAsync(query);

        // Assert
        var quotesList = result.ToList();
        for (int i = 1; i < quotesList.Count; i++)
        {
            Assert.True(string.Compare(quotesList[i - 1].QuoteNumber, quotesList[i].QuoteNumber, StringComparison.Ordinal) <= 0);
        }
    }

    [Fact]
    public async void GetQuotesAsync_WhenNoResults_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new QuoteQueryObject
        {
            PageSize = 5,
            Search = "NonExistingSearchTerm"
        };

        // Act
        var result = await _quoteService.GetQuotesAsync(query);

        // Assert
        Assert.Empty(result);
    }
    [Fact]
    public async void CreateQuoteAsync_ShouldCreateAndReturnNewQuote()
    {
        // Arrange
        var customerId = _context.Customers.First().Id;
        var machineryId = _context.Machineries.First().Id;
        var newQuoteDto = new QuoteDto.Create
        {
            CustomerId = customerId,
            MachineryId = machineryId,
            IsApproved = false,
            NewQuoteId = 0,
            Date = DateTime.UtcNow,
            QuoteNumber = "Q-12345",
            BasePrice = 1000,
            TotalWithoutVat = 1000,
            TotalWithVat = 1200,
            SalespersonId = "salesperson-id",
            MainOptions = new List<MainOptionDto>
        {
            new MainOptionDto
            {
                Category = "Category1",
                Options = new List<string> { "Option1", "Option2" }
            }
        }
        };

        // Act
        var result = await _quoteService.CreateQuoteAsync(newQuoteDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Q-12345", result.QuoteNumber);
        Assert.Equal(customerId, result.Customer.Id);
        Assert.Equal(machineryId, result.Machinery.Id);
        Assert.False(result.IsApproved);
    }


    [Fact]
    public async void CreateQuoteAsync_ShouldThrowIfCustomerNotFound()
    {
        // Arrange
        var newQuoteDto = new QuoteDto.Create
        {
            CustomerId = 99999,
            MachineryId = _context.Machineries.First().Id,
            IsApproved = false,
            NewQuoteId = 0,
            Date = DateTime.UtcNow,
            QuoteNumber = "Q-12345",
            BasePrice = 1000,
            TotalWithoutVat = 1000,
            TotalWithVat = 1200,
            SalespersonId = "salesperson-id"
        };

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _quoteService.CreateQuoteAsync(newQuoteDto));
    }

    [Fact]
    public async void CreateQuoteAsync_ShouldThrowIfMachineryNotFound()
    {
        // Arrange
        var newQuoteDto = new QuoteDto.Create
        {
            CustomerId = _context.Customers.First().Id,
            MachineryId = 99999, 
            IsApproved = false,
            NewQuoteId = 0,
            Date = DateTime.UtcNow,
            QuoteNumber = "Q-12345",
            TotalWithoutVat = 1000,
            TotalWithVat = 1200,
            SalespersonId = "salesperson-id"
        };

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _quoteService.CreateQuoteAsync(newQuoteDto));
    }

    [Fact]
    public async Task ApproveQuote_ShouldUpdateIsApproved_WhenQuoteExists()
    {
        // Arrange
        var existingQuote = await _context.Quotes.Where(x => !x.IsApproved).FirstAsync();

        // Act
        await _quoteService.ApproveQuote(existingQuote.QuoteNumber);

        // Assert
        var updatedQuote = await _context.Quotes.SingleOrDefaultAsync(q => q.QuoteNumber == existingQuote.QuoteNumber);
        Assert.NotNull(updatedQuote);
        Assert.True(updatedQuote.IsApproved);
    }

    [Fact]
    public async Task ApproveQuote_ShouldThrowIfQuoteNotFound()
    {
        // Arrange
        var quoteNumber = "Q-99999"; 

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _quoteService.ApproveQuote(quoteNumber));
    }

    [Fact]
    public async Task UpdateNewQuoteIdAsync_ShouldUpdateNewQuoteId_WhenBothQuotesExist()
    {
        // Arrange
        var quoteNumber = _context.Quotes.First().QuoteNumber;
        var newQuoteNumber = "Q-2";

        var newQuote = new Quote
        {
            QuoteNumber = newQuoteNumber,
            IsApproved = false,
            Date = DateTime.UtcNow,
            TotalWithoutVat = 1000,
            TotalWithVat = 1200,
            BasePrice = 1000,
            Customer = _context.Customers.First(),
            Machinery = _context.Machineries.First(),
            SalespersonId = "salesperson-id",
            MainOptions = "mainoptions"
        };

        await _context.Quotes.AddAsync(newQuote);
        await _context.SaveChangesAsync();

        // Act
        await _quoteService.UpdateNewQuoteIdAsync(quoteNumber, newQuoteNumber);

        // Assert
        var updatedQuote = await _context.Quotes.SingleOrDefaultAsync(x => x.QuoteNumber == quoteNumber);
        Assert.NotNull(updatedQuote);
        Assert.Equal(newQuote.Id, updatedQuote.NewQuoteId);
    }


    [Fact]
    public async Task UpdateNewQuoteIdAsync_ShouldThrowIfQuoteNotFound()
    {
        // Arrange
        var quoteNumber = "Q-12345";  
        var newQuoteNumber = _context.Quotes.First().QuoteNumber;

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _quoteService.UpdateNewQuoteIdAsync(quoteNumber, newQuoteNumber));
    }

    [Fact]
    public async Task UpdateNewQuoteIdAsync_ShouldThrowIfNewQuoteNotFound()
    {
        // Arrange
        var quoteNumber = _context.Quotes.First().QuoteNumber;
        var newQuoteNumber = "Q-99999";  


        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _quoteService.UpdateNewQuoteIdAsync(quoteNumber, newQuoteNumber));
    }

    [Fact]
    public async Task GetQuoteAsyncAndroid_ShouldReturnQuoteForValidQuoteNumber()
    {
        // Arrange
        var quote = _context.Quotes.First();
        _userService
            .Setup(service => service.GetSalesPersonAsync(quote.SalespersonId))
            .ReturnsAsync(new UserDto.Index
            {
                Email = "test",
                Name = "test",
                PhoneNumber = "test",
                Location = new LocationDto.Index
                {
                    Id = 1,
                    Name = "test",
                    Code = "test",
                    VatNumber = "test",
                    Street = "test",
                    StreetNumber = "test",
                    City = "test",
                    PostalCode = "test",
                    Country = "test",
                    Image = "test",
                    PhoneNumber = "test"
                }
            });


        // Act
        var result = await _quoteService.GetQuoteAsyncAndroid(quote.QuoteNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(quote.Id, result.Id);
        Assert.Equal(quote.QuoteNumber, result.QuoteNumber);
        Assert.Equal(quote.Customer.Name, result.Customer.Name);
    }

    [Fact]
    public async Task GetQuoteAsyncAndroid_ShouldThrowIfQuoteDoesNotExist()
    {
        // Arrange
        var quoteNumber = "99999999999";

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _quoteService.GetQuoteAsyncAndroid(quoteNumber));
    }


    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
