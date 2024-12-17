using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Domain.Exceptions;
using Rise.Shared.Helpers;
using Rise.Shared.Quotes;
using Serilog;

namespace Rise.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuoteOptionController(IQuoteOptionService quoteOptionService) : ControllerBase
{
    private readonly IQuoteOptionService quoteOptionService = quoteOptionService;

    [HttpGet]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<QuoteOptionDto.Index>> Get()
    {
        var quoteOptions = await quoteOptionService.GetQuoteOptionsAsync();
        Log.Information("Quote options retrieved");
        return quoteOptions;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<QuoteOptionDto.Index> Get(int id)
    {
        var quoteOption = await quoteOptionService.GetQuoteOptionAsync(id);
        Log.Information("Quote option retrieved by id");
        return quoteOption;
    }

    [HttpGet("ByQuote/{quoteNumber}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<QuoteOptionDto.Index>> GetByQuoteNumber(string quoteNumber)
    {
        var quoteOptions = await quoteOptionService.GetQuoteOptionsByQuoteNumberAsync(quoteNumber);
        Log.Information("Quote options retrieved by quote number");
        return quoteOptions;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<ActionResult> Delete(int id)
    {
        await quoteOptionService.DeleteQuoteOptionAsync(id);
        Log.Information("Quote option deleted");
        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<ActionResult<QuoteOptionDto.Index>> Post(QuoteOptionDto.Create quoteOption)
    {   
        Log.Information("Creating quote option");
        return await quoteOptionService.CreateQuoteOptionAsync(quoteOption);
    }
}
