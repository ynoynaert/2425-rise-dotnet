using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Shared.Machineries;
using Rise.Client.Files;
using Blazorise;
using Rise.Client.Machineries.Components;

namespace Rise.Client.Machineries.Machinery;

public partial class AddMachinery
{
	public MachineryDto.Create Model = new();
	private List<IFileEntry> images = new();
	[Inject] public required IMachineryService MachineryService { get; set; }
	[Inject] public required NavigationManager NavigationManager { get; set; }
	[Inject] public required IMachineryTypeService MachineryTypeService { get; set; }
	[Inject] public required IStorageService StorageService { get; set; }

	private IEnumerable<MachineryTypeDto.Index>? types = [];
	private string? errorMessages;
	private ConfirmationModal? confirmationModal;
	private FilePicker? filePickerCustom;
	private Validations fluentValidations = new();

	protected override async Task OnInitializedAsync()
	{
		errorMessages = null;
		try
		{
			types = await MachineryTypeService.GetMachineryTypesAsync();
		}
		catch (Exception ex)
		{
			errorMessages = "Er is een fout opgetreden tijdens het ophalen van data: " + ex.Message;
		}
	}

	void OnFileUpload(FileChangedEventArgs e)
	{
		images = e.Files.ToList();
		Model.ImageContentType = images.Select(f => f.Type).ToList();

		fluentValidations.ValidateAll();

	}

	private bool imgerror = false;
	private void ValidateFilePicker(ValidatorEventArgs e)
	{
		if (images == null || !images.Any())
		{
			imgerror = true;
			e.Status = ValidationStatus.Error;
			e.ErrorText = "Gelieve minstens één afbeelding te uploaden.";
		}
		else
		{
			imgerror = false;
			e.Status = ValidationStatus.Success;
		}
	}

	async Task ValidateData(EditContext context)
	{
		errorMessages = null;
		try
		{
			if (fluentValidations.AllValidationsSuccessful)
			{
				MachineryResult.Create result = await MachineryService.CreateMachineryAsync(Model);

				for (int i = 0; i < images.Count; i++)
				{
					var image = images[i];
					var uploadUri = result.UploadUris.ElementAtOrDefault(i);
					await StorageService.UploadImageAsync(uploadUri!, image);
				}
				NavigationManager.NavigateTo($"/machines/{result.Id}");
			}
			else
			{
				errorMessages = "Gelieve alle verplichte velden in te vullen!";
			}
		}
		catch (Exception ex)
		{
			errorMessages = "Er is een fout opgetreden tijdens het aanmaken van een machine: " + ex.Message;
		}
	}

	void OnButtonClicked()
	{
		NavigationManager.NavigateTo($"/machines");
	}

	private async Task ShowDeleteConfirmation()
	{
		await confirmationModal!.ShowAsync($"Ben je zeker dat je alle foto's wil verwijderen?");
	}
	private void OnDeleteConfirmed(bool isConfirmed)
	{
		if (isConfirmed)
		{
			filePickerCustom!.Clear();
		}
	}
}