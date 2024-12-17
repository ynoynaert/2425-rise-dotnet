using Auth0.ManagementApi.Models;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;
using System.Security.Claims;

namespace Rise.Client.Quotes;

public partial class AddOptions
{
    [Parameter]
    public string QuoteNumber { get; set; } = string.Empty;
    [Parameter]
    public OptionQueryObject Query { get; set; } = new OptionQueryObject();

    [Inject] public required IMachineryService MachineryService { get; set; }
    [Inject] public required IQuoteService QuoteService { get; set; }
    [Inject] public required IQuoteOptionService QuoteOptionService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required INotificationService NotificationService { get; set; }
    [Inject] public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    private IEnumerable<IGrouping<string, MachineryOptionDto.XtremeDetail>>? GroupedOptions;
    private Dictionary<int, bool> optionSelectionStates = new Dictionary<int, bool>();
    private string? errorMessages;
    private List<QuoteOptionDto.Index> existingOptionIds = new List<QuoteOptionDto.Index>();
    private QuoteDto.ExcelModel quote = new QuoteDto.ExcelModel();

    protected override async Task OnInitializedAsync()
    {
        await RenderItems();
    }

    private void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/offertes/{QuoteNumber}");
    }

    private async Task OnSaveButtonClicked()
    {
        errorMessages = null;
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        var selectedOptions = optionSelectionStates
            .Where(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .ToList();        

        try
        {
            var parts = quote.QuoteNumber!.Split('-');
            var newQuoteNumber = "";
            if (parts.Length == 2 && int.TryParse(parts[1], out int version))
            {
                version++;
                newQuoteNumber = $"{parts[0]}-{version}";
            }

            QuoteDto.Create updatedQuote = new QuoteDto.Create
            {
                QuoteNumber = newQuoteNumber,
                MachineryId = quote.MachineId,
                CustomerId = quote.Customer.Id,
                Date = DateTime.Today,                
                BasePrice = quote.BasePrice ?? 0m,
                TopText = quote.TopText,
                BottomText = quote.BottomText,
                TotalWithoutVat = quote.BasePrice ?? 0m,
                TotalWithVat = quote.BasePrice * 1.21M ?? 0m,
                IsApproved = false,
                SalespersonId = quote.SalespersonId!,
                MainOptions = quote.MainOptions,
            };

            var newQuote = await QuoteService.CreateQuoteAsync(updatedQuote);

            var optionsToAdd = selectedOptions.ToList();

            foreach (var optionId in optionsToAdd)
            {
                var quoteOptionDto = new QuoteOptionDto.Create
                {
                    QuoteId = newQuote.Id,
                    MachineryOptionId = optionId
                };

                await QuoteOptionService.CreateQuoteOptionAsync(quoteOptionDto);
            }

          
            await QuoteService.UpdateNewQuoteIdAsync(quote.QuoteNumber, newQuote.QuoteNumber);

            await NotificationService.Success("Opties succesvol bijgewerkt!", "Succes");
            NavigationManager.NavigateTo($"/offertes/{updatedQuote.QuoteNumber}");
        }
        catch (Exception ex)
        {
            errorMessages = $"Er is een fout opgetreden tijdens het opslaan van opties: {ex.Message}";
            await NotificationService.Error("Fout bij opslaan van opties", "Fout");
        }
    }

    private void UpdateUrl()
    {
        var queryParams = new Dictionary<string, object?>
        {
            ["Search"] = Query?.Search,
        };

        if (NavigationManager != null)
        {
            var uri = NavigationManager.GetUriWithQueryParameters(queryParams);
            NavigationManager.NavigateTo(uri);
        }
    }

    private async Task PerformFilter()
    {
        await RenderItems();
        UpdateUrl();
    }

    private async Task RenderItems()
    {
        errorMessages = null;

        try
        {
            quote = await QuoteService.GetQuoteAsync(QuoteNumber);
            var existingOptions = await QuoteOptionService.GetQuoteOptionsByQuoteNumberAsync(quote.QuoteNumber!);
            existingOptionIds = existingOptions.ToList();

            var machine = await MachineryService.GetMachineryAsyncWithCategories(quote.MachineId, Query);
            GroupedOptions = machine?.MachineryOptions?
                .Where(opt => opt.Option?.Category?.Name != null)
                .GroupBy(opt => opt.Option.Category.Name)
                .ToList()
                ?? Enumerable.Empty<IGrouping<string, MachineryOptionDto.XtremeDetail>>();

            foreach (var categoryGroup in GroupedOptions)
            {

                foreach (var option in categoryGroup)
                {
                    optionSelectionStates[option.Id] = existingOptionIds.Any(e => e.MachineryOption.Id == option.Id);
                }
            }
        }
        catch (Exception ex)
        {
            errorMessages = $"Er is een fout opgetreden tijdens het ophalen van data: {ex.Message}";
            await NotificationService.Error("Fout bij laden van data", "Fout");
        }
    }

}
