using Microsoft.AspNetCore.Components;
using Rise.Shared.Machineries;
using Rise.Client.Machineries.Components;
using Rise.Shared.Helpers;
using Rise.Client.Machineries.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Rise.Client.Machineries.Category;

public partial class CategoryTable
{
    private IEnumerable<CategoryDto.Detail>? Categories;

    [Inject] public required ICategoryService CategoryService { get; set; }

    [Inject]
    public CategoryQueryService QueryService { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Parameter]
    public CategoryQueryObject? Query { get; set; } = new CategoryQueryObject();
    private bool ExpandAll { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        Categories = await CategoryService.GetCategoriesAsync(Query!);
    }

    private void UpdateUrl()
    {
        var queryParams = new Dictionary<string, object?>
        {
            ["Search"] = Query?.Search,
        };

        if (Navigation != null)
        {
            var uri = Navigation.GetUriWithQueryParameters(queryParams);
            Navigation.NavigateTo(uri);
        }
    }

    private async Task PerformFilter()
    {
        Categories = await CategoryService.GetCategoriesAsync(Query!);
        UpdateUrl();
    }

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/categorieën/toevoegen");
    }
    void OnButtonDetailClicked(int Id)
    {
        NavigationManager.NavigateTo($"/categorieën/{Id}");
    }

    private void OnButtonEditClicked(int Id)
    {
        NavigationManager.NavigateTo($"/categorieën/{Id}/bewerken");
    }
    private void ToggleExpandAll()
    {
        ExpandAll = !ExpandAll;
    }

    void OnPriceUpdateButtonClicked()
    {
        NavigationManager.NavigateTo($"/opties/prijs-bewerken");
    }
}
