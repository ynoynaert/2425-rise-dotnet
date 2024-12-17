using Microsoft.AspNetCore.Components;
using Rise.Shared.Machineries;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Client.Machineries.Components;
using Rise.Domain.Machineries;
using Blazorise;
using Blazorise.FluentValidation;
using Rise.Shared.Helpers;

namespace Rise.Client.Machineries.MachineryOption;

public partial class EditMachineryOptions
{
    [Parameter] public int Id { get; set; }
    [Inject] public required IMachineryService MachineryService { get; set; }
    [Inject] public required IMachineryOptionService MachineryOptionService { get; set; }
    [Inject] public required ICategoryService CategoryService { get; set; }

    private MachineryOptionDto.Update? model;

    private IEnumerable<IGrouping<String, MachineryOptionDto.XtremeDetail>>? GroupedOptions;

    private int? editingOptionId = null;

    private string? errorMessages;

    private ConfirmationModal? confirmationModal;

    private int optionToDelete;

    private Validations fluentValidations = new();

    public OptionQueryObject query = new OptionQueryObject();

    protected override async Task OnInitializedAsync()
    {
       		
		    errorMessages = null;
            try
            {
                var machine = await MachineryService.GetMachineryAsyncWithCategories(Id, query);
                if (machine?.MachineryOptions != null)
                {
                    GroupedOptions = machine.MachineryOptions
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
                errorMessages = "Er is een fout opgetreden tijdens het ophalen van data: " + ex.Message;
            }
		
	}

    private bool IsEditingOption(MachineryOptionDto.XtremeDetail machineryOption) => editingOptionId == machineryOption.Id;

    private void EditOption(MachineryOptionDto.XtremeDetail machineryOption)
    {
        editingOptionId = machineryOption.Id;
        model = new MachineryOptionDto.Update
        {
            Id = machineryOption.Id,
            MachineryId = Id,
            OptionId = machineryOption.Option.Id,
            Price = machineryOption.Price,
        };
    }

    async Task SaveOption(EditContext context)
    {
	if (!fluentValidations.HasFailedValidations)
	{
		errorMessages = null;
        try
        {
            if (fluentValidations.AllValidationsSuccessful)
            {
                await MachineryOptionService.UpdateMachineryOptionAsync(model!.Id, model!);
                editingOptionId = null;
                await OnInitializedAsync();
            }
            else
            {
                errorMessages = "Gelieve alle verplichte velden in te vullen!";
            }
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het bewerken van een optie: " + ex.Message;
        }
		}

	}

    private void CancelEditOption()
    {
        editingOptionId = null;
    }

    private async Task ShowDeleteConfirmation(MachineryOptionDto.XtremeDetail machineryOption)
    {
        optionToDelete = machineryOption.Id;
        await confirmationModal!.ShowAsync($"Ben je zeker dat je {machineryOption?.Option.Code} - {machineryOption?.Option.Name} wil verwijderen?");
    }

    private async Task OnDeleteConfirmed(bool isConfirmed)
    {
        if (isConfirmed)
        {
            await DeleteOption(optionToDelete!);
        }
    }
    private async Task DeleteOption(int machineryOptionId)
    {
        errorMessages = null;
        try
        {
            await MachineryOptionService.DeleteMachineryOptionAsync(machineryOptionId);
            await OnInitializedAsync();
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het verwijderen van een optie: " + ex.Message;
        }
    }

    private void AddNewOption()
    {
        NavigationManager.NavigateTo($"/machines/{Id}/bewerken/nieuwe/opties");
    }

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/machines/{Id}");
    }

}