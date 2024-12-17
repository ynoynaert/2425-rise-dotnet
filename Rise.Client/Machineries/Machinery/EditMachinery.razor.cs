
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Machineries;
using Blazorise;
using Rise.Client.Files;
using Rise.Client.Machineries.Components;
using Blazorise.FluentValidation;


namespace Rise.Client.Machineries.Machinery;

public partial class EditMachinery
{
    [Parameter]
    public int Id { get; set; }
    public required MachineryDto.Update Model;
    [Inject] public required IMachineryService MachineryService { get; set; }
    [Inject] public required IStorageService StorageService { get; set; }
    [Inject] public required IMachineryTypeService MachineryTypeService { get; set; }

    private IEnumerable<MachineryTypeDto.Index>? types = [];
    private FilePicker? filePickerCustom;
    private ConfirmationModal? confirmationModal;
    private string? errorMessages;
    private List<ImageDto.Index>? bestaandeImages = [];
    private List<IFileEntry> images = new();
    private ImageDto.Index? todeleteImage;
    private Validations fluentValidations = new();
    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;
        try
        {
            var respons = await MachineryService.GetMachineryAsync(Id);
            bestaandeImages = respons.Images.ToList();
            Model = new MachineryDto.Update
            {
                Id = respons.Id,
                Name = respons.Name,
                SerialNumber = respons.SerialNumber,
                Description = respons.Description,
                TypeId = respons.Type.Id,
                BrochureText = respons.BrochureText,

            };
            if (bestaandeImages != null)
            {
                Model.urlOld = bestaandeImages.Select(f => f.Url).ToList();
            }
            else { Model.urlOld = new List<string>(); }
            types = await MachineryTypeService.GetMachineryTypesAsync();
        }
        catch (Exception ex)
        {
            errorMessages = "Error bij het het ophalen van de data: " + ex.Message;
        }



    }

    private bool imgerror = false;
    private void ValidateFilePicker(ValidatorEventArgs e)
    {
        if ((bestaandeImages == null || !bestaandeImages.Any()) && (images == null || !images.Any()))
        {
            imgerror = true;
            e.Status = ValidationStatus.Error;
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
                MachineryResult.Create result = await MachineryService.UpdateMachineryAsync(Model.Id, Model);

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
    private async Task ShowDeleteAllConfirmation()
    {
        await confirmationModal!.ShowAsync($"Ben je zeker dat je alle gekozen foto's wil verwijderen?");
    }
    private async Task ShowDeleteConfirmation(ImageDto.Index image)
    {
        todeleteImage = image;
        await confirmationModal!.ShowAsync($"Ben je zeker dat je deze afbeelding wil verwijderen");
    }

    private async void OnDeleteConfirmed(bool isConfirmed)
    {
        if (isConfirmed)
        {
            if (todeleteImage != null)
            {
                await OnDeleteImage();
            }
            else
            {
                await filePickerCustom!.Clear();
            }
        }
    }
    void OnFileUpload(FileChangedEventArgs e)
    {
        images = e.Files.ToList();
        Model.ImageContentTypeNew = images.Select(f => f.Type).ToList();
        fluentValidations.ValidateAll();

    }

    async Task OnDeleteImage()
    {
        bestaandeImages!.Remove(todeleteImage!);
        await MachineryService.DeleteImageMachineryAsync(Id, todeleteImage!.Id);
        Model.urlOld.Remove(todeleteImage.Url);
        await fluentValidations.ValidateAll();

    }

    async Task OnDeleteImage(ImageDto.Index image)
    {
        bestaandeImages!.Remove(image);
        await MachineryService.DeleteImageMachineryAsync(Id, image.Id);

    }
    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/machines/{Id}");
    }

}
