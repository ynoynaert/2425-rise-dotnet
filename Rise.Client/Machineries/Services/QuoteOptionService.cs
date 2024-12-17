using Rise.Shared.Helpers;
using Rise.Shared.Quotes;
using System.Net.Http;
using System.Net.Http.Json;

namespace Rise.Client.Services;

public class QuoteOptionService(HttpClient httpClient) : IQuoteOptionService
{
    public async Task<QuoteOptionDto.Index> CreateQuoteOptionAsync(QuoteOptionDto.Create quoteDto)
    {
        var response = await httpClient.PostAsJsonAsync("QuoteOption", quoteDto);
        var quoteOption = await response.Content.ReadFromJsonAsync<QuoteOptionDto.Index>();
        return quoteOption!;
    }

    public async Task DeleteQuoteOptionAsync(int id)
    {
        await httpClient.DeleteAsync($"QuoteOption/{id}");
    }

    public async Task<QuoteOptionDto.Index> GetQuoteOptionAsync(int id)
    {
        var response = await httpClient.GetAsync($"QuoteOption/{id}");
        var quoteOption = await response.Content.ReadFromJsonAsync<QuoteOptionDto.Index>();
        return quoteOption!;
    }

    public Task<IEnumerable<QuoteOptionDto.Index>> GetQuoteOptionsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<QuoteOptionDto.Index>> GetQuoteOptionsByQuoteNumberAsync(string quoteNumber)
    {
        var response = await httpClient.GetFromJsonAsync<IEnumerable<QuoteOptionDto.Index>>($"QuoteOption/ByQuote/{quoteNumber}");
        return response!;
    }
}
