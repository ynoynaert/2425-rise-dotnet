using DocumentFormat.OpenXml.Office2013.Word;
﻿using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.MachineryOption;


public partial class AddMachineryOption
{
    [Parameter] public required int MachineryId { get; set; }
    [Inject] public required IMachineryOptionService MachineryOptionService { get; set; }

    [Inject] public required IMachineryService MachineryService { get; set; }
    [Inject] public required IOptionService OptionService { get; set; }

    [Inject] public required NavigationManager NavigationManager { get; set; }

	private MachineryOptionDto.Create model = new();

    private IEnumerable<OptionDto.Detail>? beschikbareOpties = [];

    private string? errorMessages;
    private Validations fluentValidations = new();

    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;
        try
        {
            model.MachineryId = MachineryId;
            var opties = await OptionService.GetOptionsAsync();
            var machinery = await MachineryService.GetMachineryAsync(MachineryId);
            var bestaandeOpties = machinery.MachineryOptions.Select(mo => mo.OptionId);
            beschikbareOpties = opties.Where(o => !bestaandeOpties.Contains(o.Id));
		}
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het ophalen van data: " + ex.Message;
        }
    }

	async Task ValidateData(EditContext context)
    {
		errorMessages = null;
		try
		{
			if (!fluentValidations.HasFailedValidations && selectedOptionId is not null)
            {
                await MachineryOptionService.CreateMachineryOptionAsync(model);
                NavigationManager.NavigateTo($"/machines/{MachineryId}/bewerken/opties");
            }
            else
            {
                errorMessages = "Vul alle verplichte velden in!";
		    }
		}
		catch (Exception ex)
		{
			errorMessages = "Er is een fout opgetreden tijdens het aanmaken van een optie: " + ex.Message;
		}

	}

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/machines/{MachineryId}/bewerken/opties");
    }

	private OptionDto.Detail? LoadOption(int? id) => beschikbareOpties?.FirstOrDefault(o => o.Id == id);

	private int? ConvertOption(OptionDto.Detail? option) => option!.Id;
	private int? selectedOptionId
	{
		get => model.OptionId == 0 ? null : model.OptionId; 
		set => model.OptionId = value ?? 0; 
	}
	private Task<IEnumerable<OptionDto.Detail>> SearchOptions(string searchText)
	{
		if (string.IsNullOrWhiteSpace(searchText) || beschikbareOpties == null)
		{
            return Task.FromResult<IEnumerable<OptionDto.Detail>>(Array.Empty<OptionDto.Detail>());
        }

        var selectedOptions = beschikbareOpties
			.Where(option =>
				(!string.IsNullOrWhiteSpace(option.Name) && option.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
				(!string.IsNullOrWhiteSpace(option.Code) && option.Code.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
			.ToList();

        return Task.FromResult(selectedOptions.AsEnumerable());
    }
}
