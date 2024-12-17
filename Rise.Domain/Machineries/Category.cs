namespace Rise.Domain.Machineries;
public class Category : Entity
{
    private string name = default!;
    private string code = default!;
    private readonly List<Option> options = [];
    public IReadOnlyList<Option> Options => options.AsReadOnly();

    public required string Name
    {
        get => name;
        set => name = Guard.Against.NullOrWhiteSpace(value);
    }

    public required string Code
    {
        get => code;
        set => code = Guard.Against.NullOrWhiteSpace(value);
    }

    public void AddOption(Option option)
    {
        //check if exists
        if (options.Any(x => x == option))
            return;

        options.Add(option);
    }

    public void RemoveOption(Option option)
    {
        var optionToRemove = options.FirstOrDefault(x => x == option);
        if (optionToRemove is null)
            return;

        options.Remove(optionToRemove);
    }
}

