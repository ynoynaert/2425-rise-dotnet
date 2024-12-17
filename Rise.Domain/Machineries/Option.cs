namespace Rise.Domain.Machineries;
public class Option : Entity
{

    private string name = default!;
    public required string Name
    {
        get => name;
        set => name = Guard.Against.NullOrWhiteSpace(value);
    }

    private string code = default!;
    public required string Code
    {
        get => code;
        set => code = Guard.Against.NullOrWhiteSpace(value);
    }

    private Category category = default!;
    public required Category Category
    {
        get => category;
        set => category = Guard.Against.Null(value);
    }
}

