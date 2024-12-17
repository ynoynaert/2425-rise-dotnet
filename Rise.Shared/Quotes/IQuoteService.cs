using Rise.Shared.Helpers;

namespace Rise.Shared.Quotes;

public interface IQuoteService
{
    Task<bool> ApproveQuote(string quoteNumber);
    Task<IEnumerable<QuoteDto.Index>> GetQuotesAsync(QuoteQueryObject query);
    Task<QuoteDto.Android> GetQuoteForSalespersonAsyncAndroid(string quoteNumber, string userId);
    Task<QuoteDto.Android> GetQuoteAsyncAndroid(string quoteNumber);
    Task<QuoteDto.Index> CreateQuoteAsync(QuoteDto.Create quoteDto);
    Task<QuoteDto.Index> UpdateQuoteAsync(string quoteNumber, QuoteDto.Update quoteDto);
    Task UpdateNewQuoteIdAsync(string quoteNumber, string newQuoteNumber);
    Task<QuoteDto.ExcelModel> ImportFromExcelAsync(string fileBase64, string userId);
    Task<QuoteDto.ExcelModel> GetQuoteAsync(string quoteNumber);
    Task<IEnumerable<QuoteDto.Index>> GetQuotesForSalespersonAsync(string userId, QuoteQueryObject query);
    Task<QuoteDto.ExcelModel> GetQuoteForSalespersonAsync(string quoteNumber, string userId);
    Task<int> GetTotalQuotesAsync();
    Task<byte[]> GeneratePdf(string quoteNumber);
    Task<int> GetTotalQuotesForSalespersonAsync(string userId);
}
