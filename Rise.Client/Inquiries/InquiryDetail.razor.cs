using Microsoft.AspNetCore.Components;
using Rise.Domain.Quotes;
using Rise.Shared.Orders;
using Rise.Shared.Inquiries;
using static Rise.Shared.Quotes.QuoteDto;

namespace Rise.Client.Inquiries;

public partial class InquiryDetail
{
    [Parameter]
    public string Id { get; set; } = string.Empty;

    private InquiryDto.Index? Inquiry { get; set; }
    private bool isLoading = true;
    private string? errorLoading;

    [Inject]
    public required IInquiryService InquiryService { get; set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    private List<IGrouping<string, InquiryOptionDto.OptionInfo>> options = new();

    protected override async Task OnInitializedAsync()
    {
        errorLoading = null;

        try
        {
            if (!int.TryParse(Id, out var inquiryId))
            {
                throw new ArgumentException("Ongeldig Id voor offeretevoorstel");
            }

            Inquiry = await InquiryService.GetInquiryAsync(inquiryId);
            options = Inquiry.InquiryOptions.GroupBy(op => op.MachineryOption.Option.Category.Name).ToList();
        }
        catch (Exception ex)
        {
            errorLoading = $"Er is een fout opgetreden tijdens het ophalen van het offertevoorstel: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void OnButtonClicked()
    {
        NavigationManager.NavigateTo("/offertevoorstellen");
    }

    private void OnAddOptionsButtonClicked()
    {
        NavigationManager.NavigateTo($"/offertevoorstel/{Inquiry?.Id}/opties");
    }
}
