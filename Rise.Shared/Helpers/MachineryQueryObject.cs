namespace Rise.Shared.Helpers
{
    public class MachineryQueryObject
    {
        public string? Search { get; set; } = null;

        public string? TypeIds { get; set; } = null;

        public string? SortBy { get; set; } = null;

        public bool IsDescending { get; set; } = false;

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public bool HasNext { get; set; } = true;
    }
}
