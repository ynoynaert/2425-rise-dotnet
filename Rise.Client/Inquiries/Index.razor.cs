using Microsoft.AspNetCore.Components;
using Rise.Shared.Helpers;
using Rise.Shared.Inquiries;

namespace Rise.Client.Inquiries;

public partial class Index
{
    private IEnumerable<InquiryDto.Index>? inquiries;

    [Inject] public required IInquiryService InquiryService { get; set; }

    private string? errorMessages;
    //private QuoteQueryObject query = new();

    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;
        try
        {
            inquiries = await InquiryService.GetInquiriesAsync();
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een onverwachte fout opgetreden: " + ex.Message;
        }
    }
}

