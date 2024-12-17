using Blazorise;
using Microsoft.AspNetCore.Components;
using Rise.Client.Quotes;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;

namespace Rise.Client.Machineries.MachineryOption;

public partial class UpdatePrice
{
    private bool isLoading = false;

    [Inject] public required IMachineryOptionService machineryOptionService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required INotificationService NotificationService { get; set; }

    private string? errorMessages;
    private List<MachineryOptionDto.Detail> priceUpdateList = new();

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo("/categorieën");
    }

    async Task OnImportExcelFile(FileChangedEventArgs e)
    {
        var file = e.Files.FirstOrDefault();
        if (file == null) return;

        isLoading = true;
        StateHasChanged();
        errorMessages = null;
        try
        {
            using var stream = file.OpenReadStream(long.MaxValue);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);

            priceUpdateList = await machineryOptionService.ImportPriceUpdateFile(fileBase64);

        }
        catch (Exception ex)
        {
            errorMessages = $"Er is een fout opgetreden tijdens het importeren van het Excel bestand: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    async Task OnAcceptPricesButtonClicked()
    {
        errorMessages = null;
        try
        {
            foreach (var item in priceUpdateList)
            {
                var itemAsMachineryOptionUpdateDto = new MachineryOptionDto.Update
                {
                    Id = item.Id,
                    MachineryId = item.Machinery.Id,
                    OptionId = item.Option.Id,
                    Price = item.Price
                };

                await machineryOptionService.UpdateMachineryOptionAsync(itemAsMachineryOptionUpdateDto.Id, itemAsMachineryOptionUpdateDto);
            }
            await NotificationService.Success("Prijzen zijn succesvol geüpdatet");
            NavigationManager.NavigateTo("/categorieën");
        }
        catch (Exception ex)
        {
            errorMessages = $"Er is een fout opgetreden tijdens het updaten van de prijzen: {ex.Message}";
        }
    }

}
