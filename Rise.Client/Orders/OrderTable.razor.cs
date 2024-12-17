using System.Web;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Rise.Client.Quotes;
using Rise.Shared.Helpers;
using Rise.Shared.Orders;

namespace Rise.Client.Orders;

public partial class OrderTable
{
    [Parameter]
    public IEnumerable<OrderDto.Index>? Orders { get; set; }

    [Parameter]
    public OrderQueryObject Query { get; set; } = new OrderQueryObject();

    [Inject]
    public required IOrderService OrderService { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    public OrderQueryService QueryService { get; set; } = default!;

    private int totalItems;
    private bool isPreviousDisabled => Query!.PageNumber == 1;
    private bool isNextDisabled => totalItems <= Query!.PageNumber * Query!.PageSize;

    public DatePicker<DateTime?> datePickerBefore = default!;
    public DatePicker<DateTime?> datePickerAfter = default!;

    protected override async Task OnInitializedAsync()
    {
        totalItems = await OrderService.GetTotalOrders();

        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        var queryParams = HttpUtility.ParseQueryString(uri.Query);

        Query = new OrderQueryObject
        {
            Search = queryParams["Search"] ?? QueryService.SavedQuery?.Search,
            PageNumber = int.TryParse(queryParams["PageNumber"], out var pageNum) ? pageNum : QueryService.SavedQuery?.PageNumber ?? 1,
            SortBy = queryParams["SortBy"] ?? QueryService.SavedQuery?.SortBy,
            IsDescending = bool.TryParse(queryParams["IsDescending"], out var isDesc) ? isDesc : QueryService.SavedQuery?.IsDescending ?? false,
            Before = queryParams["Before"] != null
            ? DateTime.TryParse(queryParams["Before"], out var beforeDate) ? beforeDate : (DateTime?)null
            : QueryService.SavedQuery?.Before,
            After = queryParams["After"] != null
            ? DateTime.TryParse(queryParams["After"], out var afterDate) ? afterDate : (DateTime?)null
            : QueryService.SavedQuery?.After,
            Status = queryParams["Status"] ?? QueryService.SavedQuery?.Status
        };

        QueryService.SavedQuery = Query;

        Orders = await OrderService.GetOrdersAsync(Query);
    }

    private void NavigateToDetailPage(string orderNumber)
    {
        Navigation.NavigateTo($"/bestellingen/{orderNumber}");
    }

    private void UpdateUrl()
    {
        var queryParams = new Dictionary<string, object?>
        {
            ["Search"] = Query.Search,
            ["PageNumber"] = Query.PageNumber,
            ["SortBy"] = Query.SortBy,
            ["IsDescending"] = Query.IsDescending,
            ["Before"] = Query.Before?.ToString("yyyy-MM-dd"),
            ["After"] = Query.After?.ToString("yyyy-MM-dd"),
            ["Status"] = Query.Status
        };

        var uri = Navigation.GetUriWithQueryParameters(queryParams);
        Navigation.NavigateTo(uri);
    }

    private async Task PerformFilter()
    {
        Query!.PageNumber = 1;

        Orders = await OrderService.GetOrdersAsync(Query);
        UpdateUrl();
    }

    private async Task OnNextPage()
    {
        if (!isNextDisabled)
        {
            Query!.PageNumber++;
            Orders = await OrderService.GetOrdersAsync(Query);
        }
    }

    private async Task OnPreviousPage()
    {
        if (!isPreviousDisabled)
        {
            Query!.PageNumber--;
            Orders = await OrderService.GetOrdersAsync(Query);
        }
    }
}
