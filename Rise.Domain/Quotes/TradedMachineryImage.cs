namespace Rise.Domain.Quotes;

public class TradedMachineryImage : Entity
{
    private TradedMachinery tradedMachinery = default!;
    public required TradedMachinery TradedMachinery
    {
        get => tradedMachinery;
        set => tradedMachinery = Guard.Against.Null(value);
    }

    private string url = default!;
    public required string Url
    {
        get => url;
        set => url = Guard.Against.NullOrWhiteSpace(value, nameof(Url));
    }
}
