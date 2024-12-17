using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Shared.Machineries;
using Blazorise.FluentValidation;
using Blazorise;

namespace Rise.Client.Machineries.MachineryType;

public partial class AddMachineryType
{
    public MachineryTypeDto.Create Model = new();

    [Inject]
    public required IMachineryTypeService MachineryTypeService { get; set; }

    private string? errorMessages;
    private Validations fluentValidations = new();

    async Task ValidateData(EditContext context)
    {
        errorMessages = null;

        try
        {
            if (fluentValidations.AllValidationsSuccessful)
            {
                await MachineryTypeService.CreateMachineryTypeAsync(Model);
                NavigationManager.NavigateTo("/machinetypes");
            }
            else
            {
                errorMessages = "Gelieve alle verplichte velden in te vullen!";
            }
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het aanmaken van een type: " + ex.Message;
        }
    }
    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/machinetypes");
    }
}
