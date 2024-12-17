using Shouldly;
using Rise.Domain.Machineries;
using Rise.Domain.Quotes;
using Rise.Domain.Customers;

namespace Rise.Domain.Tests.Machineries;


public class MachineryOptionShould
{

    private readonly Machinery _machinery;
    private readonly Option _option;

    public MachineryOptionShould()
    {
        _machinery = new Machinery
        {
            Name = "MachineryTest1",
            SerialNumber = "123456",
            Type = new MachineryType
            {
                Name = "TypeTest1"
            },
            Description = "DescriptionTest1",
            BrochureText = "MachineryTest1 is a state-of-the-art machine designed for efficiency and durability in demanding environments. It offers exceptional performance, ensuring maximum productivity while minimizing operational costs. With advanced features and user-friendly controls, this machinery is perfect for a wide range of applications, providing reliability and high performance in every task.",
		};

        _option = new Domain.Machineries.Option
        {
            Name = "OptionTest1",
            Code = "123",
            Category = new Category
            {
                Name = "CategorieTest1",
                Code = "123a"
            }
        };
    }

    [Fact]
    public void BeCreated()
    {
        MachineryOption mo = new()
        {
            Machinery = _machinery,
            Option = _option,
            Price = 0.5M,
        };

        mo.Machinery.Name.ShouldBe("MachineryTest1");
        mo.Machinery.SerialNumber.ShouldBe("123456");
        mo.Machinery.Type.Name.ShouldBe("TypeTest1");
        mo.Machinery.Description.ShouldBe("DescriptionTest1");
        mo.Option.Name.ShouldBe("OptionTest1");
        mo.Option.Code.ShouldBe("123");
        mo.Option.Category.Name.ShouldBe("CategorieTest1");
        mo.Option.Category.Code.ShouldBe("123a");
        mo.Price.ShouldBe(0.5M);
    }




    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-1)]
    public void NotBeCreatedWithAnInvalidPrice(decimal price)
    {
        Action act = () =>
        {
            MachineryOption mo = new()
            {
                Machinery = _machinery,
                Option = _option,
                Price = price,
            };
        };
        act.ShouldThrow<ArgumentException>();
    }
}


