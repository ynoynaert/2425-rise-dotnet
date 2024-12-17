namespace Rise.Shared.Helpers;

public class UserQueryObject
{
    public string? Search { get; set; } = null;
    public string? LocationIds { get; set; } = null;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool HasNext { get; set; } = true;
}
