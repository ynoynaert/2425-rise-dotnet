using Blazorise.DeepCloner;
using Microsoft.AspNetCore.Components;
using Rise.Domain.Translations;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;
using Rise.Shared.Translations;
using System.Collections.Generic;
using System.Web;

namespace Rise.Client.Translations;

public partial class Index
{
    private IEnumerable<TranslationDto.Index>? Translations;

    private int totalItems;

    private int? editingTranslationId { get; set; } = null;

    [Parameter]
    public UnacceptedTranslationQueryObject? Query { get; set; }

    [Inject]
    public required ITranslationService TranslationService { get; set; }

    [Inject]
    public UnacceptedTranslationQueryService QueryService { get; set; } = default!;
    
    [Inject]
    public NavigationManager Navigation { get; set; } = default!;
    private string? errorMessages;
    private bool isLoading;
    private UnacceptedTranslationQueryObject query = new UnacceptedTranslationQueryObject();

    private bool isPreviousDisabled => Query!.PageNumber == 1;
    private bool isNextDisabled => totalItems <= Query!.PageNumber * Query!.PageSize;



    protected override async Task OnInitializedAsync()
    {
        totalItems = await TranslationService.GetTotalUnacceptedTranslationsAsync();
        if (Navigation != null)
            {
                var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
                var queryParams = HttpUtility.ParseQueryString(uri.Query);

                Query = new UnacceptedTranslationQueryObject
                {
                    PageNumber = int.TryParse(queryParams["PageNumber"], out var pageNum) ? pageNum : QueryService.SavedQuery?.PageNumber ?? 1
                };

                QueryService.SavedQuery = Query;
            }
        await LoadTranslationsAsync();
    }

    private async Task LoadTranslationsAsync()
    {
        isLoading = true;
        errorMessages = null;
        try
        {
            if (Query != null)
            {
                query.PageNumber = Query.PageNumber;
                query.PageSize = Query.PageSize;
            }

            Translations = await TranslationService.GetUnacceptedTranslationsAsync(query);
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
        if (Query != null)
        {
            Query.PageNumber = 1;
        }

        await LoadTranslationsAsync();
        UpdateUrl();
    }

    void OnEditButtonClicked(int id)
    {
        editingTranslationId = id;
    }

    async void OnAcceptButtonClicked(TranslationDto.Index translation)
    {
        translation.IsAccepted = true;
        await TranslationService.UpdateTranslationAsync(translation, "userEmail");
        await NotificationService.Success("Vertaling geaccepteerd!", "Succes");
        await LoadTranslationsAsync();
    }

    async Task OnSaveButtonClicked(TranslationDto.Index translation)
    {
        editingTranslationId = null;
        translation.IsAccepted = true;
        await TranslationService.UpdateTranslationAsync(translation, "test");
        await NotificationService.Success("Vertaling opgeslagen!", "Succes");
        await LoadTranslationsAsync();
    }

    async Task OnCancelButtonClicked()
    {
        editingTranslationId = null;
        await NotificationService.Warning("Bewerken geannuleerd", "Waarschuwing");
    }

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/vertalingen/overzicht");
    }

    private async Task OnNextPage()
    {
        if (!isNextDisabled)
        {
            Query!.PageNumber++;
            QueryService.SavedQuery = Query;
            await LoadTranslationsAsync();
            UpdateUrl();
        }
    }
    private async Task OnPreviousPage()
    {
        if (!isPreviousDisabled)
        {
            Query!.PageNumber--;
            QueryService.SavedQuery = Query;
            await LoadTranslationsAsync();
            UpdateUrl();
        }
    }
}
