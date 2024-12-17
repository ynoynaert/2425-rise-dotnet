namespace Rise.Domain.Translations;

public class Translation : Entity
{
    private bool isAccepted = default!;

    public required bool IsAccepted
    {
        get => isAccepted;
        set => isAccepted = value;
    }

    private string originalText = default!;

    public required string OriginalText
    {
        get => originalText;
        set => originalText = Guard.Against.NullOrWhiteSpace(value);
    }

    private string translatedText = default!;

    public required string TranslatedText
    {
        get => translatedText;
        set => translatedText = Guard.Against.NullOrWhiteSpace(value);
    }

    private string? userEmail = default!;
	public string? UserEmail
	{
		get => userEmail;
		set => userEmail = Guard.Against.NullOrWhiteSpace(value);
	}
}
