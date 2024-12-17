using Microsoft.AspNetCore.Components;
using Rise.Shared.Helpers;
using Rise.Shared.Quotes;

namespace Rise.Client.Quotes;

public partial class Index
{
    private IEnumerable<QuoteDto.Index>? quotes;

    [Inject] public required IQuoteService QuoteService { get; set; }

    private string? errorMessages;
    private QuoteQueryObject query = new();

    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;
        try
        {
            quotes = await QuoteService.GetQuotesAsync(query);
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een onverwachte fout opgetreden: " + ex.Message;
        }
    }

    void OnCreateQuoteButtonClicked()
    {
        NavigationManager.NavigateTo($"/offertes/toevoegen");
    }
}

