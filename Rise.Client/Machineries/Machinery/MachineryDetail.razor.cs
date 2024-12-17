using Microsoft.AspNetCore.Components;
using Rise.Client.Machineries.Components;
using Rise.Client.Machineries.Services;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.Machinery;

public partial class MachineryDetail
{
    [Parameter]
    public int Id { get; set; }

    [Inject]
    public required IMachineryService MachineryService { get; set; }

    [Inject]
    public required MachineryQueryService QueryService { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;
    public OptionQueryObject query = new OptionQueryObject();
    private IEnumerable<IGrouping<String, MachineryOptionDto.XtremeDetail>>? GroupedOptions;
    private MachineryDto.XtremeDetail? Machine;
    private ConfirmationModal? confirmationModal;
    private string? errorMessages;

    private string? SelectedImage;

    private void SelectImage(string image)
    {
		SelectedImage = image;
        StateHasChanged();
	}

    private void OnButtonClicked()
    {
        var query = QueryService.SavedQuery;

        var queryParams = new Dictionary<string, object?>
        {
            ["Search"] = query?.Search,
            ["PageNumber"] = query?.PageNumber,
            ["TypeIds"] = query?.TypeIds,
            ["SortBy"] = query?.SortBy,
            ["IsDescending"] = query?.IsDescending
        };

        var uri = Navigation.GetUriWithQueryParameters("/machines", queryParams);

        Navigation.NavigateTo(uri);
    }

    private async Task ShowDeleteConfirmation()
    {
        await confirmationModal!.ShowAsync($"Ben je zeker dat je {Machine?.Name} wil verwijderen?");
    }

    private async Task OnDeleteConfirmed(bool isConfirmed)
    {
        if (isConfirmed)
        {
            await OnDeleteButtonClicked();
        }
    }

    private async Task OnDeleteButtonClicked()
    {
        errorMessages = null;
        try
        {
            await MachineryService.DeleteMachineryAsync(Id);
            Navigation.NavigateTo($"/machines");

        }
        catch (Exception ex)
        {
            errorMessages = ex.Message;
        }

    }

    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;
        try
        {
            Machine = await MachineryService.GetMachineryAsyncWithCategories(Id, query);
			SelectedImage = Machine.Images.FirstOrDefault()?.Url;
			if (Machine?.MachineryOptions != null)
            {
                GroupedOptions = Machine.MachineryOptions
                    .Where(opt => opt.Option?.Category?.Name != null)
                    .GroupBy(opt => opt.Option.Category.Code)
                    .ToList();
            }
            else
            {
                GroupedOptions = Enumerable.Empty<IGrouping<String, MachineryOptionDto.XtremeDetail>>();
            }
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het ophalen van de machine: " + ex.Message;
        }

    }

    void OnButtonEditClicked(int id)
    {
        Navigation.NavigateTo($"/machines/{id}/bewerken");
    }

    void OnOptionEditClicked(int id)
    {
        Navigation.NavigateTo($"/machines/{id}/bewerken/opties");
    }

}
