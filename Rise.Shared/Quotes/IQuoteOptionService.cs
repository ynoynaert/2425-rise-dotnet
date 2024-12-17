using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Rise.Shared.Helpers;

namespace Rise.Shared.Quotes;

public interface IQuoteOptionService
{
    Task<IEnumerable<QuoteOptionDto.Index>> GetQuoteOptionsAsync();
    Task<QuoteOptionDto.Index> GetQuoteOptionAsync(int id);
    Task<QuoteOptionDto.Index> CreateQuoteOptionAsync(QuoteOptionDto.Create quoteDto);
    Task<IEnumerable<QuoteOptionDto.Index>> GetQuoteOptionsByQuoteNumberAsync(string quoteNumber);
    Task DeleteQuoteOptionAsync(int id);
}
