using Rise.Shared.Users;

namespace Rise.Shared.Translations;

public static class TranslationDto
{
    public class Index
    {
        public int Id { get; set; }
        public required string OriginalText { get; set; }
        public required string TranslatedText { get; set; }
        public required bool IsAccepted { get; set; }
        public string? UserEmail { get; set; }
	}
}
