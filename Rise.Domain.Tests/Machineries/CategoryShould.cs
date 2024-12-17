using Rise.Domain.Machineries;
using Shouldly;

namespace Rise.Domain.Tests.Machineries;

public class CategoryShould
{
    [Fact]
    public void BeCreated()
    {
        Category c = new()
        {
            Name = "TestCategory 1",
            Code = "1200"
        };

        c.Name.ShouldBe("TestCategory 1");
        c.Code.ShouldBe("1200");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidName(string? name)
    {
        Action act = () =>
        {
            Category c = new()
            {
                Name = name!,
                Code = "1200"
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidCode(string? code)
    {
        Action act = () =>
        {
            Category c = new()
            {
                Name = "TestCategory 1",
                Code = code!
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
        Category c = new()
        {
            Name = "TestCategory 1",
            Code = "1200"
        };

        Action act = () =>
        {
            c.Name = name!;
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeChangedToAnInvalidCode(string? code)
    {
        Category c = new()
        {
            Name = "TestCategory 1",
            Code = "1200"
        };

        Action act = () =>
        {
            c.Code = code!;
        };
        act.ShouldThrow<ArgumentException>();
    }
}
