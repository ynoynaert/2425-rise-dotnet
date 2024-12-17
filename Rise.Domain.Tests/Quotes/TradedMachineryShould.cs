using Rise.Domain.Customers;
using Rise.Domain.Machineries;
using Rise.Domain.Quotes;
using Shouldly;

namespace Rise.Domain.Tests.Quotes;

public class TradedMachineryShould
{
    private readonly MachineryType _machineryType;
    private readonly Quote _quote;

    public TradedMachineryShould()
    {
        _machineryType = new MachineryType { Name = "Graafmachine" };

        _quote = new Quote
        {
            QuoteNumber = "1234",
            IsApproved = false,
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
            BasePrice = 1000m,
            TotalWithoutVat = 1000m,
            TotalWithVat = 1200m,
            Machinery = new Machinery
            {
                Name = "Caterpillar 320",
                SerialNumber = "CAT320-202310001",
                Type = _machineryType,
                Description = "Graafmachine voor zwaar werk", 
                BrochureText = "De Caterpillar 320 graafmachine is ontworpen voor zware bouwprojecten waar efficiëntie en betrouwbaarheid van cruciaal belang zijn. Deze graafmachine biedt een ongeëvenaarde combinatie van kracht, duurzaamheid en brandstofbesparing dankzij de nieuwste hydraulische technologieën. De 320 is uitgerust met een geavanceerd controlesysteem waarmee operators complexe taken moeiteloos kunnen uitvoeren. Met het geavanceerde hydraulische systeem, een comfortabel ontwerp, en verminderde uitstoot is de Caterpillar 320 perfect voor langdurige graaf- en sloopwerkzaamheden, zelfs in uitdagende omgevingen. Dit model verhoogt de productiviteit en minimaliseert de operationele kosten, met speciale voorzieningen voor onderhoudsgemak en duurzaamheid in veeleisende omgevingen."
            }
        };
    }

    [Fact]
    public void BeCreatedSuccessfully()
    {
        var tradedMachinery = new TradedMachinery
        {
            Name = "Kubota KX040-4",
            Type = _machineryType,
            SerialNumber = "KUBKX040-001",
            Description = "Compacte graafmachine voor veelzijdige toepassingen",
            EstimatedValue = 50000m,
            Year = 2020,
            Quote = _quote
        };

        tradedMachinery.Name.ShouldBe("Kubota KX040-4");
        tradedMachinery.Type.Name.ShouldBe("Graafmachine");
        tradedMachinery.SerialNumber.ShouldBe("KUBKX040-001");
        tradedMachinery.Description.ShouldBe("Compacte graafmachine voor veelzijdige toepassingen");
        tradedMachinery.EstimatedValue.ShouldBe(50000m);
        tradedMachinery.Year.ShouldBe(2020);
        tradedMachinery.Quote.QuoteNumber.ShouldBe("1234");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidName(string? name)
    {
        Action act = () =>
        {
            new TradedMachinery
            {
                Name = name!,
                Type = _machineryType,
                SerialNumber = "KUBKX040-001",
                Description = "Test",
                EstimatedValue = 50000m,
                Year = 2020,
                Quote = _quote
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidType(MachineryType? type)
    {
        Action act = () =>
        {
            new TradedMachinery
            {
                Name = "Test",
                Type = type!,
                SerialNumber = "KUBKX040-001",
                Description = "Test",
                EstimatedValue = 50000m,
                Year = 2020,
                Quote = _quote
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NotBeCreatedWithAnInvalidEstimatedValue(decimal estimatedValue)
    {
        Action act = () =>
        {
            new TradedMachinery
            {
                Name = "Test",
                Type = _machineryType,
                SerialNumber = "KUBKX040-001",
                Description = "Test",
                EstimatedValue = estimatedValue,
                Year = 2020,
                Quote = _quote
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NotBeCreatedWithAnInvalidYear(int year)
    {
        Action act = () =>
        {
            new TradedMachinery
            {
                Name = "Test",
                Type = _machineryType,
                SerialNumber = "KUBKX040-001",
                Description = "Test",
                EstimatedValue = 50000m,
                Year = year,
                Quote = _quote
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void AllowAddingAndRemovingImages()
    {
        var tradedMachinery = new TradedMachinery
        {
            Name = "Kubota KX040-4",
            Type = _machineryType,
            SerialNumber = "KUBKX040-001",
            Description = "Compacte graafmachine voor veelzijdige toepassingen",
            EstimatedValue = 50000m,
            Year = 2020,
            Quote = _quote
        };

        var image1 = new TradedMachineryImage { TradedMachinery = tradedMachinery, Url = "http://image1.com" };
        var image2 = new TradedMachineryImage { TradedMachinery = tradedMachinery, Url = "http://image2.com" };

        tradedMachinery.AddImage(image1);
        tradedMachinery.AddImage(image2);

        tradedMachinery.Images.ShouldContain(image1);
        tradedMachinery.Images.ShouldContain(image2);

        tradedMachinery.RemoveImage(image1);

        tradedMachinery.Images.ShouldNotContain(image1);
        tradedMachinery.Images.ShouldContain(image2);
    }
}
