using System.Web;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Helpers;
using Rise.Shared.Inquiries;

namespace Rise.Client.Inquiries;

public partial class InquiryTable
{
    [Parameter]
    public IEnumerable<InquiryDto.Index>? Inquiries { get; set; }
    /*[Parameter]
    public QuoteQueryObject Query { get; set; } = new QuoteQueryObject();*/

    [Inject]
    public required IInquiryService InquiryService { get; set; }
    [Inject]
    public NavigationManager Navigation { get; set; } = default!;
    /*[Inject]
    public InquiryQueryService InqueryService { get; set; } = default!;*/

   /* private int totalItems;
    private bool isPreviousDisabled => Query!.PageNumber == 1;
    private bool isNextDisabled => totalItems <= Query!.PageNumber * Query!.PageSize;*/

/*    public DatePicker<DateTime?> datePickerBefore = default!;
    public DatePicker<DateTime?> datePickerAfter = default!;
*/
/*    protected override async Task OnInitializedAsync()
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
                : QueryService.SavedQuery?.After
        };

        QueryService.SavedQuery = Query;
        Quotes = await QuoteService.GetQuotesAsync(Query);
    }*/

    protected override async Task OnInitializedAsync()
    {
        Inquiries = await InquiryService.GetInquiriesAsync();
    }

  /*  private void UpdateUrl()
    {

        var queryParams = new Dictionary<string, object?>
        {
            ["Search"] = Query.Search,
            ["PageNumber"] = Query.PageNumber,
            ["SortBy"] = Query.SortBy,
            ["IsDescending"] = Query.IsDescending,
            ["Before"] = Query.Before?.ToString("yyyy-MM-dd"),
            ["After"] = Query.After?.ToString("yyyy-MM-dd")
        };

        var uri = Navigation.GetUriWithQueryParameters(queryParams);
        Navigation.NavigateTo(uri);
    }*/

    private void NavigateToDetailPage(int id)
    {
        Navigation.NavigateTo($"/offertevoorstel/{id}");
    }

/*    private async Task PerformFilter()
    {
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
    }*/

}
