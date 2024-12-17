using Rise.Domain.Inquiries;
using Rise.Domain.Machineries;
using Shouldly;

namespace Rise.Domain.Tests.Inquiries;

    public class InquiriesShould
{
    private readonly Machinery _machinery;

    public InquiriesShould()
    {
        _machinery = new Machinery
        {
            Name = "Caterpillar 320",
            SerialNumber = "CAT320-202310001",
            Type = new MachineryType { Name = "Graafmachine" },
            Description = "Caterpillar 320 - Graafmachine",
            BrochureText = "De Caterpillar 320 graafmachine is ontworpen voor zware bouwprojecten..."
        };
    }

    [Fact]
    public void BeCreated()
    {
        Inquiry i = new Inquiry
        {
            CustomerName = "Test Customer",
            Machinery = _machinery,
            SalespersonId = "1234"
        };

        i.CustomerName.ShouldBe("Test Customer");
        i.Machinery.Name.ShouldBe("Caterpillar 320");
        i.SalespersonId.ShouldBe("1234");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithInvalidCustomerName(string? customerName)
    {
        Action act = () =>
        {
            Inquiry i = new Inquiry
            {
                CustomerName = customerName!,
                Machinery = _machinery,
                SalespersonId = "1234"
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithInvalidSalespersonId(string? salespersonId)
    {
        Action act = () =>
        {
            Inquiry i = new Inquiry
            {
                CustomerName = "Test Customer",
                Machinery = _machinery,
                SalespersonId = salespersonId!
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void NotBeCreatedWithNullMachinery()
    {
        Action act = () =>
        {
            Inquiry i = new Inquiry
            {
                CustomerName = "Test Customer",
                Machinery = null!,
                SalespersonId = "1234"
            };
        };
        act.ShouldThrow<ArgumentException>();
    }
}