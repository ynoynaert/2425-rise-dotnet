namespace Rise.Shared.Helpers
{
    public class OrderQueryObject
    {
        public string? Search { get; set; } = null;

        public DateTime? Before { get; set; } = null;
        public DateTime? After { get; set; } = null;

        public string? SortBy { get; set; } = null;

        public bool IsDescending { get; set; } = false;

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public bool HasNext { get; set; } = true;

        public string? Status { get; set; } = null;
    }
}
