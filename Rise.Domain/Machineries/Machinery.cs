using Rise.Domain.Exceptions;
using Rise.Domain.Quotes;

namespace Rise.Domain.Machineries;
public class Machinery : Entity
{
    private string name = default!;
    public required string Name
    {
        get => name;
        set => name = Guard.Against.NullOrWhiteSpace(value);
    }

    private string serialNumber = default!;
    public required string SerialNumber
    {
        get => serialNumber;
        set => serialNumber = Guard.Against.NullOrWhiteSpace(value);
    }

    private MachineryType type = default!;
    public required MachineryType Type
    {
        get => type;
        set => type = Guard.Against.Null(value);
    }

    private string description = default!;
    public required string Description
    {
        get => description;
        set => description = Guard.Against.NullOrWhiteSpace(value);
    }

    private string brochureText = default!;
    public required string BrochureText
    {
        get => brochureText;
        set => brochureText = Guard.Against.NullOrWhiteSpace(value);
    }

    private readonly List<Image> images = [];
    public IReadOnlyList<Image> Images => images.AsReadOnly();

	public void AddImage(Image image)
	{
		images.Add(image);
	}
    public void RemoveImage(Image image)
	{
		images.Remove(image);
	}

	private readonly List<MachineryOption> machineryOptions = [];
    public IReadOnlyList<MachineryOption> MachineryOptions => machineryOptions.AsReadOnly();

    public MachineryOption AddMachineryOption(Option option, decimal price, Quote quote)
    {
        //check if exists
        if (machineryOptions.Any(x => x.Option == option))
            throw new EntityAlreadyExistsException("Optie", "id", option.Id);

        var machineryOption = new MachineryOption
        {
            Machinery = this,
            Option = option,
            Price = price,
        };

        machineryOptions.Add(machineryOption);
        return machineryOption;
    }

    public void RemoveMachineryOption(Option option)
    {
        var machineryOption = machineryOptions.FirstOrDefault(x => x.Option == option);
        if (machineryOption is not null)
        {
            machineryOptions.Remove(machineryOption); 
        }
    }
}

