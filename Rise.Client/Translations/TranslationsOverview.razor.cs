using Microsoft.AspNetCore.Components;
using Rise.Shared.Helpers;
using Rise.Shared.Translations;
using System.Web;

namespace Rise.Client.Translations;

public partial class TranslationsOverview
{
    private IEnumerable<TranslationDto.Index>? Translations;

    [Parameter]
    public TranslationQueryObject? Query { get; set; }

    private int totalItems;

    private int? editingTranslationId { get; set; } = null;

    [Inject]
    public required ITranslationService TranslationService { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    public TranslationQueryService QueryService { get; set; } = default!;

    private bool isPreviousDisabled => Query!.PageNumber == 1;
    private bool isNextDisabled => totalItems <= Query!.PageNumber * Query!.PageSize;

    private string? errorMessages;
    private bool isLoading;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            totalItems = await TranslationService.GetTotalAcceptedTranslationsAsync();

            if (Navigation != null)
            {
                var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
                var queryParams = HttpUtility.ParseQueryString(uri.Query);

                Query = new TranslationQueryObject
                {
                    Search = queryParams["Search"] ?? QueryService.SavedQuery?.Search,
                    PageNumber = int.TryParse(queryParams["PageNumber"], out var pageNum) ? pageNum : QueryService.SavedQuery?.PageNumber ?? 1
                };

                QueryService.SavedQuery = Query;
            }

            await LoadTranslationsAsync();
        }
        catch (Exception ex)
        {
            errorMessages = $"Initialization error: {ex.Message}";
        }
    }

    private async Task LoadTranslationsAsync()
    {
        isLoading = true;
        errorMessages = null;
        try
        {
            if (TranslationService != null && Query != null)
            {
                Translations = await TranslationService.GetAcceptedTranslationsAsync(Query);
            }
        }
        catch (Exception ex)
        {
            errorMessages = "Error bij het ophalen van de data: " + ex.Message;
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void UpdateUrl()
    {
        var queryParams = new Dictionary<string, object?>
        {
            ["Search"] = Query?.Search,
            ["PageNumber"] = Query?.PageNumber
        };

        if (Navigation != null)
        {
            var uri = Navigation.GetUriWithQueryParameters(queryParams);
            Navigation.NavigateTo(uri);
        }
    }

    private async Task PerformFilter()
    {
        Query!.PageNumber = 1;


        await LoadTranslationsAsync();
        UpdateUrl();
    }


    void OnEditButtonClicked(int id)
    {
        editingTranslationId = id;
    }

    async Task OnSaveButtonClicked(TranslationDto.Index translation)
    {
        editingTranslationId = null;
        translation.IsAccepted = true;

		if (TranslationService != null)
        {
            await TranslationService.UpdateTranslationAsync(translation, "userEmail");
            await NotificationService.Success("Vertaling opgeslagen!", "Succes");
            await LoadTranslationsAsync();
        }
    }

    async Task OnCancelButtonClicked()
    {
        editingTranslationId = null;
        await NotificationService.Warning("Bewerken geannuleerd", "Waarschuwing");
    }

    void OnButtonClicked()
    {
        if (Navigation != null)
        {
            Navigation.NavigateTo("/vertalingen");
        }
    }

    private async Task OnNextPage()
        {
            if (!isNextDisabled)
            {
                Query!.PageNumber++;
                await LoadTranslationsAsync();
                UpdateUrl();            }
        }
        private async Task OnPreviousPage()
        {
            if (!isPreviousDisabled)
            {
                Query!.PageNumber--;
                await LoadTranslationsAsync();
                UpdateUrl();
            }
        }
}
