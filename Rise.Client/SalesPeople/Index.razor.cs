using Microsoft.AspNetCore.Components;
using Rise.Shared.Helpers;
using Rise.Shared.Locations;
using Rise.Shared.Users;
using System.Web;

namespace Rise.Client.SalesPeople;

public partial class Index
{
    private IEnumerable<UserDto.Index>? salesPeople;
    private IEnumerable<LocationDto.Index>? locations;
    private IReadOnlyList<int> filteredLocations { get; set; } = [];

    public UserQueryObject? Query { get; set; } = new UserQueryObject();

    [Inject]
    public required IUserService UserService { get; set; }
    [Inject] 
    public UserQueryService QueryService { get; set; } = default!;
    [Inject] 
    public NavigationManager Navigation { get; set; } = default!;
    [Inject]    
    public required ILocationService LocationService { get; set; }

    private string? errorMessages;
    private int totalItems;
    private bool isPreviousDisabled => Query!.PageNumber == 1;
    private bool isNextDisabled => totalItems <= Query!.PageNumber * Query!.PageSize;

    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;

        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        var queryParams = HttpUtility.ParseQueryString(uri.Query);

        Query = new UserQueryObject
        {
            Search = queryParams["Search"] ?? QueryService.SavedQuery?.Search,
            LocationIds = queryParams["LocationIds"] ?? QueryService.SavedQuery?.LocationIds,
            PageNumber = int.TryParse(queryParams["PageNumber"], out var pageNum) ? pageNum : QueryService.SavedQuery?.PageNumber ?? 1,
        };

        if (!string.IsNullOrEmpty(Query.LocationIds))
        {
            filteredLocations = Query.LocationIds.Split('-')
                                          .Select(int.Parse)
                                          .ToList();
        }

        QueryService.SavedQuery = Query;

        try
        {
            salesPeople = await UserService.GetSalesPeopleAsync(Query);
            totalItems = await UserService.GetTotalSalesPeopleAsync();
            locations = await LocationService.GetLocationsAsync();
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een onverwachte fout opgetreden: " + ex.Message;
        }
    }

    private async Task PerformFilter()
    {
        Query!.PageNumber = 1;
        if (filteredLocations.Count == 0)
        {
            Query!.LocationIds = null;
        }
        else
        {
            Query!.LocationIds = string.Join('-', filteredLocations);
        }

        salesPeople = await UserService.GetSalesPeopleAsync(Query!);

        UpdateUrl();
    }

    private void UpdateUrl()
    {
        var queryParams = new Dictionary<string, object?>
        {
            ["Search"] = Query?.Search,
            ["LocationIds"] = Query?.LocationIds,
            ["PageNumber"] = 1
        };

        var uri = Navigation.GetUriWithQueryParameters(queryParams);
        Navigation.NavigateTo(uri);
    }
    private async Task OnNextPage()
    {
        if (!isNextDisabled)
        {
            Query!.PageNumber++;
            salesPeople = await UserService.GetSalesPeopleAsync(Query);
        }
    }
    private async Task OnPreviousPage()
    {
        if (!isPreviousDisabled)
        {
            Query!.PageNumber--;
            salesPeople = await UserService.GetSalesPeopleAsync(Query);
        }
    }

    private void OnSignUpSalesPersonClicked()
    {
        QueryService.SavedQuery = Query;
        Navigation.NavigateTo("/verkopers/toevoegen");
    }
}
