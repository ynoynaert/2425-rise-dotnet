using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Shared.Machineries;
using Blazorise.FluentValidation;
using Blazorise;

namespace Rise.Client.Machineries.MachineryType;

public partial class EditMachineryType
{
    public required MachineryTypeDto.Update Model;

    [Parameter]
    public int Id { get; set; }

    [Inject]
    public required IMachineryTypeService MachineryTypeService { get; set; }

    private string? errorMessages;
    private Validations fluentValidations = new();

    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;
        try
        {
            var category = await MachineryTypeService.GetMachineryTypeAsync(Id);
            Model = new MachineryTypeDto.Update
            {
                Id = category.Id,
                Name = category.Name!,
            };
        }
        catch (Exception ex)
        {
            errorMessages = "Error bij het het ophalen van de data: " + ex.Message;
        }
    }

    async Task ValidateData(EditContext context)
    {
        errorMessages = null;

        try
        {
            if (fluentValidations.AllValidationsSuccessful)
            {
                await MachineryTypeService.UpdateMachineryTypeAsync(Id, Model);
                NavigationManager.NavigateTo("/machinetypes");
            }
            else
            {
                errorMessages = "Gelieve alle verplichte velden in te vullen!";
            }
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het bewerken van een type: " + ex.Message;
        }
    }
    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/machinetypes");
    }
}
