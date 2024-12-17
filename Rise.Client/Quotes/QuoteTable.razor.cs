using System.Web;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rise.Shared.Quotes;

namespace Rise.Client.Quotes;

public partial class QuoteTable
{
    [Parameter]
    public IEnumerable<QuoteDto.Index>? Quotes { get; set; }
    [Parameter]
    public QuoteQueryObject Query { get; set; } = new QuoteQueryObject();

    [Inject]
    public required IQuoteService QuoteService { get; set; }
    [Inject] 
    public NavigationManager Navigation { get; set; } = default!;
    [Inject] 
    public QuoteQueryService QueryService { get; set; } = default!;

    private int totalItems;
    private bool isPreviousDisabled => Query!.PageNumber == 1;
    private bool isNextDisabled => totalItems <= Query!.PageNumber * Query!.PageSize;
    
    public DatePicker<DateTime?> datePickerBefore = default!;
    public DatePicker<DateTime?> datePickerAfter = default!;

    protected override async Task OnInitializedAsync()
    {
        totalItems = await QuoteService.GetTotalQuotesAsync();

        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        var queryParams = HttpUtility.ParseQueryString(uri.Query);
      
        Query = new QuoteQueryObject
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
        Quotes = await QuoteService.GetQuotesAsync(Query);
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
  
    private void NavigateToDetailPage(string id)
    {
        Navigation.NavigateTo($"/offertes/{id}");
    }

    private async Task PerformFilter()
    {
        Query!.PageNumber = 1;

        Quotes = await QuoteService.GetQuotesAsync(Query);
        UpdateUrl();
    }

    private async Task OnNextPage()
    {
        if (!isNextDisabled)
        {
            Query!.PageNumber++;
            Quotes = await QuoteService.GetQuotesAsync(Query);
        }
    }
  
    private async Task OnPreviousPage()
    {
        if (!isPreviousDisabled)
        {
            Query!.PageNumber--;
            Quotes = await QuoteService.GetQuotesAsync(Query);
        }
    }

}
