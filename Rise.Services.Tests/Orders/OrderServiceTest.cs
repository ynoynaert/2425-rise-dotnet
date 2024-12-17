using Auth0.ManagementApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using QuestPDF.Fluent;
using Rise.Domain.Exceptions;
using Rise.Domain.Orders;
using Rise.Domain.Quotes;
using Rise.Persistence;
using Rise.Services.Customers;
using Rise.Services.Documents;
using Rise.Services.Files;
using Rise.Services.Machineries;
using Rise.Services.Orders;
using Rise.Services.Quotes;
using Rise.Services.Translations;
using Rise.Services.Users;
using Rise.Shared.Documents;
using Rise.Shared.Helpers;
using Rise.Shared.Orders;
using Rise.Shared.Quotes;
using Xunit;

namespace Rise.Services.Tests.Orders;

public class OrderServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly OrderService _orderService;
    private readonly IQuoteService _quoteService;
    private readonly IConfiguration _configuration;
    private readonly IStorageService _storageService;
    private readonly TranslationService _translationService;
    private readonly CustomerService _customerService;
    private readonly MachineryService _machineryService;
    private readonly UserService _userService;
    private readonly Mock<IManagementApiClient> _mockManagementApiClient;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly IDocumentService _documentService;

    public OrderServiceTest()
    {
        _mockLogger = new Mock<ILogger<OrderService>>();
        _context = TestApplicationDbContextFactory.CreateDbContext();
        _context.Database.Migrate();
        Seeder seeder = new Seeder(_context);
        seeder.Seed();

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
        _mockManagementApiClient = new Mock<IManagementApiClient>();
        _userService = new UserService(_context, _mockManagementApiClient.Object);
        _documentService = new DocumentService(_userService);
        _quoteService = new QuoteService(_context, _translationService, _customerService, _machineryService, _userService, _documentService);
        _orderService = new OrderService(_context, _quoteService, _userService, _documentService, _mockLogger.Object);

    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateAndReturnNewOrder()
    {
        // Arrange
        var quote = _context.Quotes.Include(q => q.Customer).First();
        var orderDto = new OrderDto.Create
        {
            QuoteId = quote.Id,
            OrderNumber = "ORD-12345",
            Date = DateTime.UtcNow
        };

        // Act
        var result = await _orderService.CreateOrderAsync(orderDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderDto.OrderNumber, result.OrderNumber);
        Assert.Equal(quote.Id, result.Quote.Id);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrowIfQuoteNotFound()
    {
        // Arrange
        var orderDto = new OrderDto.Create
        {
            QuoteId = 99999, 
            OrderNumber = "ORD-12345",
            Date = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _orderService.CreateOrderAsync(orderDto));
    }

    [Fact]
    public async Task GetOrderAsync_ShouldReturnOrderById()
    {
        // Arrange
        var order = _context.Orders.First();

        // Act
        var result = await _orderService.GetOrderAsync(order.OrderNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
    }

    [Fact]
    public async Task GetOrderAsync_ShouldThrowIfOrderNotFound()
    {
        // Arrange
        var invalidOrderNumber = "99999";

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _orderService.GetOrderAsync(invalidOrderNumber));
    }

    [Fact]
    public async Task GetTotalOrders_ShouldReturnTotalOrderCount()
    {
        // Act
        var result = await _orderService.GetTotalOrders();

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public async Task CheckIfQuoteHasOrder_ShouldReturnTrue_IfOrderExists()
    {
        // Arrange
        var existingOrder = await _context.Orders.FirstAsync();

        string orderNumber = existingOrder.OrderNumber;

        // Act
        var result = await _orderService.CheckIfQuoteHasOrder(orderNumber);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CheckIfQuoteHasOrder_ShouldReturnFalse_IfOrderDoesNotExist()
    {
        // Arrange
        string nonExistingOrderNumber = "999999";

        // Act
        var result = await _orderService.CheckIfQuoteHasOrder(nonExistingOrderNumber);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateOrderAsync_ShouldThrowIfOrderNumberConflictOnUpdate()
    {
        // Arrange
        var existingOrder = _context.Orders.Include(o => o.Quote).First();
        var conflictingOrder = new Order
        {
            OrderNumber = "123456789",
            Quote = existingOrder.Quote,
            Date = DateTime.UtcNow.AddDays(1),
        };
        _context.Orders.Add(conflictingOrder);
        _context.SaveChanges();

        var orderDto = new OrderDto.Update
        {
            OrderNumber = conflictingOrder.OrderNumber,  
            Quote = new QuoteDto.ExcelModel { Id = existingOrder.Quote.Id },
            Date = DateTime.UtcNow.AddDays(1),
            IsCancelled = false
        };

        // Act & Assert
        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _orderService.UpdateOrderAsync(existingOrder.OrderNumber, orderDto));
    }

    [Fact]
    public async Task UpdateOrderAsync_ShouldUpdateOrder()
    {
        // Arrange
        var existingOrder = _context.Orders.Include(o => o.Quote).First();
        var customer = _context.Customers.First();
        var machinery = _context.Machineries.First();
        var orderDto = new OrderDto.Update
        {
            OrderNumber = "1111",
            Quote = new QuoteDto.ExcelModel { Id = 2 },
            Date = DateTime.UtcNow.AddDays(1),
            IsCancelled = true
        };

        // Act
        await _orderService.UpdateOrderAsync(existingOrder.OrderNumber, orderDto);

        // Assert
        var updatedOrder = _context.Orders.Include(o => o.Quote).First(o => o.OrderNumber == orderDto.OrderNumber);
        Assert.NotNull(updatedOrder);
        Assert.Equal(orderDto.OrderNumber, updatedOrder.OrderNumber);
        Assert.Equal(orderDto.Date, updatedOrder.Date);
        Assert.Equal(orderDto.IsCancelled, updatedOrder.IsCancelled);
        Assert.Equal(2, updatedOrder.Quote.Id);
    }


    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
