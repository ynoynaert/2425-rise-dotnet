using System.Web;
using Microsoft.AspNetCore.Components;
using Rise.Client.Machineries.Services;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.Machinery
{
    public partial class MachineryTable
    {
        [Parameter]
        public IEnumerable<MachineryDto.Detail>? Machinery { get; set; }

        [Parameter]
        public MachineryQueryObject? Query { get; set; }

        [Parameter]
        public IEnumerable<MachineryTypeDto.Index>? MachineryTypeDtos { get; set; }
        private int totalItems;

        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public required IMachineryService MachineService { get; set; }
        [Inject] public MachineryQueryService QueryService { get; set; } = default!;

        private bool isPreviousDisabled => Query!.PageNumber == 1;
        private bool isNextDisabled => totalItems <= Query!.PageNumber * Query!.PageSize;
        private IReadOnlyList<int> filteredTypes { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {

            totalItems = await MachineService.GetTotalMachineriesAsync();

            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);

            Query = new MachineryQueryObject
            {
                Search = queryParams["Search"] ?? QueryService.SavedQuery?.Search,
                PageNumber = int.TryParse(queryParams["PageNumber"], out var pageNum) ? pageNum : QueryService.SavedQuery?.PageNumber ?? 1,
                TypeIds = queryParams["TypeIds"] ?? QueryService.SavedQuery?.TypeIds,
                SortBy = queryParams["SortBy"] ?? QueryService.SavedQuery?.SortBy,
                IsDescending = bool.TryParse(queryParams["IsDescending"], out var isDesc) ? isDesc : QueryService.SavedQuery?.IsDescending ?? false
            };

            if (!string.IsNullOrEmpty(Query.TypeIds))
            {
                filteredTypes = Query.TypeIds.Split('-')
                                              .Select(int.Parse)
                                              .ToList();
            }

            QueryService.SavedQuery = Query;
            Machinery = await MachineService.GetMachineriesAsync(Query);
        }

        private void UpdateUrl()
        {
            var queryParams = new Dictionary<string, object?>
            {
                ["Search"] = Query?.Search,
                ["PageNumber"] = Query?.PageNumber,
                ["TypeIds"] = Query?.TypeIds,
                ["SortBy"] = Query?.SortBy,
                ["IsDescending"] = Query?.IsDescending
            };

            var uri = Navigation.GetUriWithQueryParameters(queryParams);
            Navigation.NavigateTo(uri);
        }
        private async Task PerformFilter()
        {
            Query!.PageNumber = 1;
            Query.TypeIds = filteredTypes.Any() ? string.Join("-", filteredTypes) : null;

            Machinery = await MachineService.GetMachineriesAsync(Query);

            UpdateUrl();
        }

        private async Task OnNextPage()
        {
            if (!isNextDisabled)
            {
                Query!.PageNumber++;
                Machinery = await MachineService.GetMachineriesAsync(Query);
            }
        }
        private async Task OnPreviousPage()
        {
            if (!isPreviousDisabled)
            {
                Query!.PageNumber--;
                Machinery = await MachineService.GetMachineriesAsync(Query);
            }
        }

        private void NavigateToDetailPage(int id)
        {
            QueryService.SavedQuery = Query;
            Navigation.NavigateTo($"/machines/{id}");
        }
    }
}
