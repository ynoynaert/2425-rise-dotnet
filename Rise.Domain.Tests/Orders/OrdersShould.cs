using Rise.Domain.Customers;
using Rise.Domain.Machineries;
using Rise.Domain.Orders;
using Rise.Domain.Quotes;
using Shouldly;

namespace Rise.Domain.Tests.Orders;

public class OrdersShould
{
    private readonly Quote _quote;
    private readonly DateTime _date;

    public OrdersShould()
    {
        _quote = new Quote
        {
            QuoteNumber = "1234",
            IsApproved = true,
            NewQuoteId = 1,
            Date = DateTime.Now,
            Customer = new Customer
            {
                Name = "Test Customer",
                Street = "Test Street",
                StreetNumber = "1",
                City = "Test City",
                PostalCode = "1234",
                Country = "Test Country"
            },
            TotalWithoutVat = 1000m,
            TotalWithVat = 1200m,
            Machinery = new Machinery
            {
                Name = "Caterpillar 320",
                SerialNumber = "CAT320-202310001",
                Type = new MachineryType { Name = "Graafmachine" },
                Description = "Caterpillar 320 - Graafmachine",
                BrochureText = "brochure text"
            }
        };

        _date = DateTime.Now;
    }

    [Fact]
    public void BeCreated()
    {
        Order order = new Order
        {
            OrderNumber = "ORD-5678",
            Quote = _quote,
            Date = _date
        };

        order.OrderNumber.ShouldBe("ORD-5678");
        order.Quote.ShouldBe(_quote);
        order.Date.ShouldBe(_date);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidOrderNumber(string? orderNumber)
    {
        Action act = () =>
        {
            Order order = new Order
            {
                OrderNumber = orderNumber!,
                Quote = _quote,
                Date = _date
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void NotBeCreatedWithAnInvalidQuote()
    {
        Action act = () =>
        {
            Order order = new Order
            {
                OrderNumber = "ORD-5678",
                Quote = null!,
                Date = _date
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void NotBeCreatedWithADefaultDate()
    {
        Action act = () =>
        {
            Order order = new Order
            {
                OrderNumber = "ORD-5678",
                Quote = _quote,
                Date = default
            };
        };

        act.ShouldThrow<ArgumentException>();
    }
}
