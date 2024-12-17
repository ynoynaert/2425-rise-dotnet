using Shouldly;
using Rise.Domain.Machineries;

namespace Rise.Domain.Tests.Machineries;

public class OptionShould
{
    private readonly Category _category;

    public OptionShould()
    {
        _category = new Category
        {
            Name = "CategorieTest1",
            Code = "123a"
        };
    }

    [Fact]
    public void BeCreated()
    {
        Option o = new()
        {
            Name = "OptionTest1",
            Code = "123",
            Category = _category,
        };

        o.Name.ShouldBe("OptionTest1");
        o.Code.ShouldBe("123");
        o.Category.Name.ShouldBe("CategorieTest1");
        o.Category.Code.ShouldBe("123a");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidName(string? name)
    {
        Action act = () =>
        {
            Option o = new()
            {
                Name = name!,
                Code = "123",
                Category = _category,
            };
        };
        act.ShouldThrow<ArgumentException>();
    }

}
