using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rise.Client.Machineries.Components;
using Rise.Shared.Machineries;
using System.Reflection.PortableExecutable;

namespace Rise.Client.Machineries.MachineryType;

public partial class TypeTable
{
    [Inject] public required IMachineryTypeService MachineryTypeService { get; set; }

    private IEnumerable<MachineryTypeDto.Index>? MachineryTypes { get; set; }

    private ConfirmationModal? confirmationModal;

    private string? errorMessages;

    private int typeToDelete;

    protected override async Task OnInitializedAsync()
    {
        MachineryTypes = await MachineryTypeService.GetMachineryTypesAsync();
    }

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/machinetypes/toevoegen");
    }

    private void Edit(int id)
    {
        NavigationManager.NavigateTo($"/machinetypes/{id}/bewerken");
    }

    private async Task ShowDeleteConfirmation(MachineryTypeDto.Index machineryType)
    {
        typeToDelete = machineryType.Id;
        await confirmationModal!.ShowAsync($"Ben je zeker dat je {machineryType.Name} wil verwijderen?");
    }

    private async Task OnDeleteConfirmed(bool isConfirmed)
    {
        if (isConfirmed)
        {
            await OnDeleteButtonClicked(typeToDelete);
        }
    }

    private async Task OnDeleteButtonClicked(int typeToDelete)
    {
        errorMessages = null;
        try
        {
            await MachineryTypeService.DeleteMachineryTypeAsync(typeToDelete);
            MachineryTypes = await MachineryTypeService.GetMachineryTypesAsync();

        }
        catch (Exception ex)
        {
            errorMessages = ex.Message;
        }

    }
}
