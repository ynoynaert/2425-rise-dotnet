using Rise.Domain.Quotes;

namespace Rise.Domain.Machineries;
public class MachineryOption : Entity
{

    private Machinery machinery = default!;
    public required Machinery Machinery
    {
        get => machinery;
        set => machinery = Guard.Against.Null(value);
    }

    private Option option = default!;
    public required Option Option
    {
        get => option;
        set => option = Guard.Against.Null(value);
    }

    private decimal price;
    public required decimal Price
    {
        get => price;
        set => price = Guard.Against.NegativeOrZero(value);
    }

}

