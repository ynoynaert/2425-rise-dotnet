using Blazorise;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Quotes;

namespace Rise.Client.Quotes;

public partial class LoadExcel
{
    private bool isLoading = false;

    [Inject]
    public required IQuoteService QuoteService { get; set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    private string? errorMessages;

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo("/offertes");
    }

    async Task OnImportExcelFile(FileChangedEventArgs e)
    {
        var file = e.Files.FirstOrDefault();
        if (file == null) return;

        isLoading = true;
        StateHasChanged();
        try
        {
            using var stream = file.OpenReadStream(long.MaxValue);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);    

            var excelModel = await QuoteService.ImportFromExcelAsync(fileBase64, "");

            NavigationManager.NavigateTo($"/offertes/{excelModel.QuoteNumber}");
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
}
