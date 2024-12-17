using Microsoft.AspNetCore.Components;
using Rise.Persistence.Migrations;
using Rise.Shared.Locations;

namespace Rise.Client.Locations;

public partial class LocationTable
{
    [Parameter] public IEnumerable<LocationDto.Detail>? Locations { get; set; }

    [Inject] public NavigationManager Navigation { get; set; } = default!;

    [Inject] public ILocationService LocationService { get; set; } = default!;

	private string? errorMessages;

	protected override async Task OnInitializedAsync()
    {
		errorMessages = null;
		try
		{
			Locations = await LocationService.GetLocationsWithSalesPeopleAsync();
		}
		catch (Exception ex)
		{
			errorMessages = $"Er is een fout opgetreden bij het laden van de locaties: {ex.Message}";
		}
	}
}
