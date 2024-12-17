
using Rise.Domain.Machineries;

namespace Rise.Domain.Quotes;

public class TradedMachinery : Entity
{
    private string name = default!;
    public required string Name
    {
        get => name;
        set => name = Guard.Against.NullOrWhiteSpace(value);
    }

    private MachineryType type = default!;
    public required MachineryType Type
    {
        get => type;
        set => type = Guard.Against.Null(value);
    }

    private string serialNumber = default!;
    public required string SerialNumber
    {
        get => serialNumber;
        set => serialNumber = Guard.Against.NullOrWhiteSpace(value);
    }

    private string description = default!;
    public required string Description
    {
        get => description;
        set => description = Guard.Against.NullOrWhiteSpace(value);
    }

    private decimal estimatedValue = default!;
    public required decimal EstimatedValue
    {
        get => estimatedValue;
        set => estimatedValue = Guard.Against.NegativeOrZero(value);
    }

    private int year = default!;
    public required int Year
    {
        get => year;
        set => year = Guard.Against.NegativeOrZero(value);
    }

    private Quote quote = default!;
    public required Quote Quote
    {
        get => quote;
        set => quote = Guard.Against.Null(value);
    }

    private readonly List<TradedMachineryImage> images = [];
    public IReadOnlyList<TradedMachineryImage> Images => images.AsReadOnly();

    public void AddImage(TradedMachineryImage image)
    {
        images.Add(image);
    }
    public void RemoveImage(TradedMachineryImage image)
    {
        images.Remove(image);
    }

}
