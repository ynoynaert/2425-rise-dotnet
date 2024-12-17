using Microsoft.AspNetCore.Components;
using Rise.Shared.Locations;

namespace Rise.Client.Locations;

public partial class Index
{
    private IEnumerable<LocationDto.Detail>? locations;
    private string? errorMessages;
    [Inject] public required ILocationService LocationService { get; set; }

    protected override async Task OnInitializedAsync()
    {
		errorMessages = null;
		try
        {
            locations = await LocationService.GetLocationsWithSalesPeopleAsync();
        }
        catch (Exception ex)
        {
            errorMessages = $"Er is een fout opgetreden bij het laden van de locaties: {ex.Message}";
        }
    }
}
