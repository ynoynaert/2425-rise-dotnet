using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Client.Files;
using Rise.Client.Machineries.Components;
using Rise.Client.Services;
using Rise.Domain.Quotes;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;

namespace Rise.Client.Quotes;

public partial class AddTradedMachinery
{
    [Parameter] public string QuoteNumber { get; set; } = string.Empty;
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IMachineryTypeService MachineryTypeService { get; set; }
	[Inject] public required ITradedMachineryService TradedMachineryService { get; set; }
	[Inject] public required IStorageService StorageService { get; set; }
	[Inject] public required IQuoteService QuoteService { get; set; }

	[Inject] public required IQuoteOptionService QuoteOptionService { get; set; }

    public TradedMachineryDto.Create Model = new();

	private QuoteDto.ExcelModel quote = new();
	private IEnumerable<MachineryTypeDto.Index>? types = [];
	private string? errorMessages;
	private string? errorAction;
	private ConfirmationModal? confirmationModal;
	private FilePicker? filePickerCustom;
	private Validations fluentValidations = new();
	private List<IFileEntry> images = new();
	protected override async Task OnInitializedAsync()
    {
		errorMessages = null;
		try {
			quote = await QuoteService.GetQuoteAsync(QuoteNumber);
			types = await MachineryTypeService.GetMachineryTypesAsync();
		}
		catch (Exception ex)
		{
			errorMessages = "Er is een fout opgetreden tijdens het ophalen van de data: " + ex.Message;
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
		errorAction = null;
		try
		{
			if (fluentValidations.AllValidationsSuccessful)
			{

                Model.QuoteNumber = quote.QuoteNumber!;
				TradedMachineryResult.Create result = await TradedMachineryService.CreateTradedMachineryAsync(Model);

				//images
				for (int i = 0; i < images.Count; i++)
				{
					var image = images[i];
					var uploadUri = result.UploadUris.ElementAtOrDefault(i);
					await StorageService.UploadImageAsync(uploadUri!, image);
				}

                NavigationManager.NavigateTo($"/offertes/{QuoteNumber}");
            }
			else
			{
				errorAction = "Gelieve alle verplichte velden in te vullen!";
			}
		}
		catch (Exception ex)
		{
			errorAction = "Er is een fout opgetreden tijdens het inruilen van een machine: " + ex.Message;
		}
	}

		private void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/offertes/{QuoteNumber}");
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