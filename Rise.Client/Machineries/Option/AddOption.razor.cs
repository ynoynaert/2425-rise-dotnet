using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Shared.Machineries;
using Blazorise.FluentValidation;
using Blazorise;

namespace Rise.Client.Machineries.Option;

partial class AddOption
{
    [Parameter]
    public int Id { get; set; }

    public OptionDto.Create Model = new();

    [Inject]
    public required IOptionService OptionService { get; set; }
    private string? errorMessages;
    private Validations fluentValidations = new();

    async Task ValidateData(EditContext context)
    {
        errorMessages = null;
        try
        {
            if (fluentValidations.AllValidationsSuccessful)
            {
                Model.CategoryId = Id;
                await OptionService.CreateOptionAsync(Model);
                NavigationManager.NavigateTo($"/categorieën/{Id}");
            }
            else
            {
                errorMessages = "Gelieve alle verplichte velden in te vullen!";
            }
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het aanmaken van een optie: " + ex.Message;
        }
    }

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/categorieën/{Id}");
    }
}