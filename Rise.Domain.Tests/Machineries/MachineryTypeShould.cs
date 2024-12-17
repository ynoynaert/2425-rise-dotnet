using Rise.Domain.Machineries;
using Shouldly;

namespace Rise.Domain.Tests.Machineries;

public class MachineryTypeShould
{
    [Fact]
    public void BeCreated()
    {
        MachineryType mt = new()
        {
            Name = "Graafmachine"
        };

        mt.Name.ShouldBe("Graafmachine");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidName(string? name)
    {
        Action act = () =>
        {
            MachineryType mt = new()
            {
                Name = name!
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeChangedToAnInvalidName(string? name)
    {
        MachineryType t = new()
        {
            Name = "TestType"
        };

        Action act = () =>
        {
            t.Name = name!;
        };

        act.ShouldThrow<ArgumentException>();
    }
}
