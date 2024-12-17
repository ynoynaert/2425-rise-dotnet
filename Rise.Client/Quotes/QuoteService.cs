using Rise.Shared.Helpers;
using Azure;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;
using System.Net.Http;
using System.Net.Http.Json;

namespace Rise.Client.Quotes;

public class QuoteService(HttpClient httpClient, ILogger<QuoteService> logger) : IQuoteService
{
    private readonly HttpClient httpClient = httpClient;
    private readonly ILogger<QuoteService> logger = logger;
    public async Task<QuoteDto.Index> CreateQuoteAsync(QuoteDto.Create quoteDto)
    {
        var response = await httpClient.PostAsJsonAsync("quote", quoteDto);
        var quote = await response.Content.ReadFromJsonAsync<QuoteDto.Index>();
        return quote!;
    }

    public async Task<IEnumerable<QuoteDto.Index>> GetQuotesAsync(QuoteQueryObject query)
    {
        string url = $"quote?Search={query.Search}&Before={query.Before?.ToString("yyyy-MM-dd")}&After={query.After?.ToString("yyyy-MM-dd")}&SortBy={query.SortBy}&IsDescending={query.IsDescending}&PageNumber={query.PageNumber}&PageSize={query.PageSize}&Status={query.Status}";
        var result = await httpClient.GetFromJsonAsync<IEnumerable<QuoteDto.Index>>(url);
        return result ?? Enumerable.Empty<QuoteDto.Index>();
    }


    public async Task<QuoteDto.ExcelModel> ImportFromExcelAsync(string fileBase64, string userId)
    {
        var fileUploadModel = new { FileBase64 = fileBase64 };
        var response = await httpClient.PostAsJsonAsync("quote/import-excel", fileUploadModel);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<QuoteDto.ExcelModel>() ?? new QuoteDto.ExcelModel();
    }

    public async Task UpdateNewQuoteIdAsync(string quoteNumber, string newQuoteNumber)
    {
        await httpClient.PatchAsync($"quote/{quoteNumber}/{newQuoteNumber}", null);
    }

    public async Task<QuoteDto.ExcelModel> GetQuoteAsync(string quoteNumber)
    {
        var response = await httpClient.GetFromJsonAsync<QuoteDto.ExcelModel>($"quote/{quoteNumber}");
        return response ?? new QuoteDto.ExcelModel();
    }

    public async Task<IEnumerable<QuoteDto.Index>> GetQuotesForSalespersonAsync(string userId, QuoteQueryObject query)
    {
        string url = $"quote?Search={query.Search}&Before={query.Before?.ToString("yyyy-MM-dd")}&After={query.After?.ToString("yyyy-MM-dd")}&SortBy={query.SortBy}&IsDescending={query.IsDescending}&PageNumber={query.PageNumber}&PageSize={query.PageSize}&Status={query.Status}";
        var result = await httpClient.GetFromJsonAsync<IEnumerable<QuoteDto.Index>>(url);
        return result ?? Enumerable.Empty<QuoteDto.Index>();

    }

    public async Task<QuoteDto.ExcelModel> GetQuoteForSalespersonAsync(string quoteNumber, string userId)
    {
        var response = await httpClient.GetFromJsonAsync<QuoteDto.ExcelModel>($"quote/{quoteNumber}");
        return response ?? new QuoteDto.ExcelModel();
    }

    public Task<int> GetTotalQuotesAsync()
    {
        return httpClient.GetFromJsonAsync<int>("quote/total");
    }

    public async Task<bool> ApproveQuote(string quoteNumber)
    {
        var response = await httpClient.PatchAsync($"quote/{quoteNumber}/approve", null);
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<byte[]> GeneratePdf(string quoteNumber)
    {
        var response = await httpClient.GetAsync($"quote/{quoteNumber}/pdf");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to generate PDF: {ErrorMessage}", errorMessage);
            throw new Exception($"Failed to generate PDF for quote {quoteNumber}: {errorMessage}");
        }

        var pdfBytes = await response.Content.ReadAsByteArrayAsync();
        logger.LogInformation("PDF bytes length: {Length}", pdfBytes.Length);

        return pdfBytes;
    }

    public Task<QuoteDto.Android> GetQuoteAsyncAndroid(string quoteNumber)
    {
        throw new NotImplementedException();
    }

    public Task<QuoteDto.Android> GetQuoteForSalespersonAsyncAndroid(string quoteNumber, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalQuotesForSalespersonAsync(string userId)
    {
        throw new NotImplementedException();

    }

    public async Task<QuoteDto.Index> UpdateQuoteAsync(string quoteNumber, QuoteDto.Update quoteDto)
    {
        var response = await httpClient.PutAsJsonAsync($"quote/{quoteNumber}", quoteDto);
        var quote = await response.Content.ReadFromJsonAsync<QuoteDto.Index>();
        return quote!;
    }
}
