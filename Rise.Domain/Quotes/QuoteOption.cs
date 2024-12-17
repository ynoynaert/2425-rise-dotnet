using Rise.Domain.Machineries;

namespace Rise.Domain.Quotes;
public class QuoteOption : Entity
{

    private Quote quote = default!;
    private MachineryOption machineryOption = default!;

    public Quote Quote
    {
        get => quote;
        set => quote = Guard.Against.Null(value);
    }

    public MachineryOption MachineryOption
    {
        get => machineryOption;
        set => machineryOption = Guard.Against.Null(value);
    }
}

