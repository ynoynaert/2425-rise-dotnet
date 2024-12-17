namespace Rise.Shared.Helpers
{
    public class TranslationQueryObject
    {
        public string? Search { get; set; } = null;

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public bool HasNext { get; set; } = true;
    }
}
