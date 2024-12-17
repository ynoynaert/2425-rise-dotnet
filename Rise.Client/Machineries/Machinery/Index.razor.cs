using Microsoft.AspNetCore.Components;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.Machinery;
        
public partial class Index
{
    private IEnumerable<MachineryDto.Detail>? machinery;

    private MachineryQueryObject query = new MachineryQueryObject();

    private IEnumerable<MachineryTypeDto.Index>? machineryTypeDtos;

    [Inject] public required IMachineryService MachineService { get; set; }
    [Inject] public required IMachineryTypeService MachineryTypeService { get; set; }

    private string? errorMessages;
    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;
        try
        {
            machinery = await MachineService.GetMachineriesAsync(query);
            machineryTypeDtos = await MachineryTypeService.GetMachineryTypesAsync();
        }catch (Exception ex)
		{
			errorMessages = "Er is een onverwachte fout opgetreden: " + ex.Message;
		}

	}

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/machines/toevoegen");
    }

}

