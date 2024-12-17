using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Client.Auth;
using Rise.Shared.Locations;
using Rise.Shared.Users;

namespace Rise.Client.SalesPeople;

public partial class AddSalesPerson
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    public required IUserService UserService { get; set; }
	[Inject]
	public required ILocationService LocationService { get; set; }

	private IEnumerable<LocationDto.Index> Locations { get; set; } = [];
	private string? errorMessages;
    public UserDto.Create Model = new();
	private Validations fluentvalidations = new();

	protected override async Task OnInitializedAsync()
	{
		errorMessages = null;
		try
		{
			Locations = await LocationService.GetLocationsAsync();
		}
		catch (Exception ex)
		{
			errorMessages = "Er is een fout opgetreden tijdens het ophalen van data: " + ex.Message;
		}
	}

	private async Task ValidateData(EditContext context)
    {
		errorMessages = null;
        try
        {
            if (fluentvalidations.AllValidationsSuccessful)
            {
                await UserService.CreateUserAsync(Model);
                NavigationManager.NavigateTo("/verkopers");
			}
			else
			{
				errorMessages = "Gelieve alle verplichte velden in te vullen!";
			}
		}
		catch (Exception ex)
		{
			errorMessages = "Er is een fout opgetreden bij het maken van een verkopersaccount: " + Auth0ErrorMapper.GetTranslatedErrorMessage(ex.Message);
		}
	}

	private void OnButtonClicked()
    {
        NavigationManager.NavigateTo("/verkopers");
    }
}