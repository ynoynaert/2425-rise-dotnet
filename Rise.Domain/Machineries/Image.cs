namespace Rise.Domain.Machineries;

public class Image : Entity
{
	private Machinery machinery = default!;
	public required Machinery Machinery
	{
		get => machinery;
		set => machinery = Guard.Against.Null(value);
	}

	private string url = default!;
	public required string Url
	{
		get => url;
		set => url = Guard.Against.NullOrWhiteSpace(value, nameof(Url));
	}
}
