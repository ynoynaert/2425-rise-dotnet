using System.Net.Http.Json;
using Rise.Shared.Inquiries;

namespace Rise.Client.Inquiries;

public class InquiryService(HttpClient httpClient) : IInquiryService
{
    public async Task<InquiryDto.Index> CreateInquiryAsync(InquiryDto.Create inquiryDto)
    {
        var response = await httpClient.PostAsJsonAsync("inquiry", inquiryDto);
        var inquiry = await response.Content.ReadFromJsonAsync<InquiryDto.Index>();
        return inquiry!;
    }

    public async Task DeleteInquiryAsync(int id)
    {
        await httpClient.DeleteAsync($"inquiry/{id}");
    }

    public async Task<IEnumerable<InquiryDto.Index>> GetInquiriesAsync()
    {
        string url = $"inquiry";
        var result = await httpClient.GetFromJsonAsync<IEnumerable<InquiryDto.Index>>(url);
        return result ?? Enumerable.Empty<InquiryDto.Index>();
    }

    public async Task<IEnumerable<InquiryDto.Index>> GetInquiriesForSalespersonAsync(string userId)
    {
        string url = $"inquiry";
        var result = await httpClient.GetFromJsonAsync<IEnumerable<InquiryDto.Index>>(url);
        return result ?? Enumerable.Empty<InquiryDto.Index>();
    }

    public async Task<InquiryDto.Index> GetInquiryAsync(int id)
    {
        var response = await httpClient.GetFromJsonAsync<InquiryDto.Index>($"inquiry/{id}");
        return response!;
    }

    public async Task<InquiryDto.Index> GetInquiryForSalespersonAsync(int id, string userId)
    {
        var response = await httpClient.GetFromJsonAsync<InquiryDto.Index>($"inquiry/{id}");
        return response!; ;
    }
}
