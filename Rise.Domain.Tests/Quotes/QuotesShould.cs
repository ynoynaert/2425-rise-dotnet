using Rise.Domain.Customers;
using Rise.Domain.Machineries;
using Rise.Domain.Quotes;
using Shouldly;

namespace Rise.Domain.Tests.Quotes;

public class QuotesShould
{
    private readonly Customer _customer;
    private readonly Machinery _machinery;
    private readonly DateTime _date;

    public QuotesShould()
    {
        _customer = new Customer
        {
            Name = "Test Customer",
            Street = "Test Street",
            StreetNumber = "1",
            City = "Test City",
            PostalCode = "1234",
            Country = "Test Country"
        };

        _machinery = new Machinery
        {
            Name = "Caterpillar 320",
            SerialNumber = "CAT320-202310001",
            Type = new MachineryType { Name = "Graafmachine" },
            Description = "Caterpillar 320 - Graafmachine",
            BrochureText = "De Caterpillar 320 graafmachine is ontworpen voor zware bouwprojecten waar efficiëntie en betrouwbaarheid van cruciaal belang zijn. Deze graafmachine biedt een ongeëvenaarde combinatie van kracht, duurzaamheid en brandstofbesparing dankzij de nieuwste hydraulische technologieën. De 320 is uitgerust met een geavanceerd controlesysteem waarmee operators complexe taken moeiteloos kunnen uitvoeren. Met het geavanceerde hydraulische systeem, een comfortabel ontwerp, en verminderde uitstoot is de Caterpillar 320 perfect voor langdurige graaf- en sloopwerkzaamheden, zelfs in uitdagende omgevingen. Dit model verhoogt de productiviteit en minimaliseert de operationele kosten, met speciale voorzieningen voor onderhoudsgemak en duurzaamheid in veeleisende omgevingen."
        };

        _date = DateTime.Now;
    }

    [Fact]
    public void BeCreated()
    {
        Quote q = new Quote
        {
            QuoteNumber = "1234",
            IsApproved = false,
            NewQuoteId = 1,
            Date = _date,
            Customer = _customer,
            BasePrice = 1000m,
            TotalWithoutVat = 1000m,
            TotalWithVat = 1200m,
            Machinery = _machinery
        };

        q.QuoteNumber.ShouldBe("1234");
        q.IsApproved.ShouldBeFalse();
        q.NewQuoteId.ShouldBe(1);
        q.Date.ShouldBe(_date);
        q.Customer.Name.ShouldBe("Test Customer");
        q.BasePrice.ShouldBe(1000m);
        q.TotalWithoutVat.ShouldBe(1000m);
        q.TotalWithVat.ShouldBe(1200m);
        q.Machinery.Name.ShouldBe("Caterpillar 320");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidQuoteNumber(string? quoteNumber)
    {
        Action act = () =>
        {
            Quote q = new Quote
            {
                QuoteNumber = quoteNumber!,
                IsApproved = false,
                NewQuoteId = 1,
                Date = _date,
                Customer = _customer,
                BasePrice = 1000m,
                TotalWithoutVat = 1000m,
                TotalWithVat = 1200m,
                Machinery = _machinery
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-1)]
    public void NotBeCreatedWithAnInvalidTotalWithoutVat(decimal totalWithoutVat)
    {
        Action act = () =>
        {
            Quote q = new Quote
            {
                QuoteNumber = "1234",
                IsApproved = false,
                NewQuoteId = 1,
                Date = _date,
                Customer = _customer,
                BasePrice = 1000m,
                TotalWithoutVat = totalWithoutVat,
                TotalWithVat = 1200m,
                Machinery = _machinery
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-1)]
    public void NotBeCreatedWithAnInvalidTotalWithVat(decimal totalWithVat)
    {
        Action act = () =>
        {
            Quote q = new Quote
            {
                QuoteNumber = "1234",
                IsApproved = false,
                NewQuoteId = 1,
                Date = _date,
                Customer = _customer,
                BasePrice = 1000m,
                TotalWithoutVat = 1000m,
                TotalWithVat = totalWithVat,
                Machinery = _machinery
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidCustomer(Customer? customer)
    {
        Action act = () =>
        {
            Quote q = new Quote
            {
                QuoteNumber = "1234",
                IsApproved = false,
                NewQuoteId = 1,
                Date = _date,
                Customer = customer!,
                BasePrice = 1000m,
                TotalWithoutVat = 1000m,
                TotalWithVat = 1200m,
                Machinery = _machinery
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidMachinery(Machinery? machinery)
    {
        Action act = () =>
        {
            Quote q = new Quote
            {
                QuoteNumber = "1234",
                IsApproved = false,
                NewQuoteId = 1,
                Date = _date,
                Customer = _customer,
                BasePrice = 1000m,
                TotalWithoutVat = 1000m,
                TotalWithVat = 1200m,
                Machinery = machinery!
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

	[Theory]
	[InlineData(null)]
	public void NotBeCreatedWithAnInvalidDate(DateTime? date)
	{
		Action act = () =>
		{
			Quote q = new Quote
			{
				QuoteNumber = "1234",
				IsApproved = false,
				NewQuoteId = 1,
				Date = date ?? throw new ArgumentException("Invalid date"),
				Customer = _customer,
				BasePrice = 1000m,
				TotalWithoutVat = 1000m,
				TotalWithVat = 1200m,
				Machinery = _machinery
			};
		};
		act.ShouldThrow<ArgumentException>();
	}

	[Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-1)]
    public void NotBeCreatedWithAnInvalidBasePrice(decimal basePrice)
    {
        Action act = () =>
        {
            Quote q = new Quote
            {
                QuoteNumber = "1234",
                IsApproved = false,
                NewQuoteId = 1,
                Date = _date,
                Customer = _customer,
                BasePrice = basePrice,
                TotalWithoutVat = 1000m,
                TotalWithVat = 1200m,
                Machinery = _machinery
            };
        };
        act.ShouldThrow<ArgumentException>();
    }
}
