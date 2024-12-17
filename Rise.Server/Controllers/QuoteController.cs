using System.Security.Claims;
using DocumentFormat.OpenXml.Office2010.Word;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Domain.Exceptions;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;
using Serilog;

namespace Rise.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuoteController(IQuoteService quoteService) : ControllerBase
{
    private readonly IQuoteService quoteService = quoteService;

    [HttpGet]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<QuoteDto.Index>> Get([FromQuery] QuoteQueryObject query)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        if (User.IsInRole("Verkoper"))
        {
            Log.Information("Quotes retrieved for salesperson");
            return await quoteService.GetQuotesForSalespersonAsync(userId, query);
        }

        Log.Information("Quotes retrieved for admin");
        return await quoteService.GetQuotesAsync(query);  
    }

    [HttpGet("{quoteNumber}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<ActionResult<QuoteDto.ExcelModel>> GetByQuoteNumber(string quoteNumber)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        if (User.IsInRole("Verkoper"))
        {
            Log.Information("Quote retrieved for salesperson");
            return Ok(await quoteService.GetQuoteForSalespersonAsync(quoteNumber, userId));
        }

        var quote = await quoteService.GetQuoteAsync(quoteNumber);
        Log.Information("Quote retrieved for admin");
        return Ok(quote);
    }

    [HttpGet("android/{quoteNumber}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<ActionResult<QuoteDto.Android>> Get(string quoteNumber)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        if (User.IsInRole("Verkoper"))
        {
            Log.Information("Quote retrieved for salesperson");
            return Ok(await quoteService.GetQuoteForSalespersonAsyncAndroid(quoteNumber, userId));
        }

        var quote = await quoteService.GetQuoteAsyncAndroid(quoteNumber);
        Log.Information("Quote retrieved for admin");
        return Ok(quote);
    }

    [HttpPatch("{quoteNumber}/{newQuoteNumber}")]
    [Authorize(Roles = "Verkoper")]
    public async Task<IActionResult> Patch(string quoteNumber, string newQuoteNumber)
    {
        await quoteService.UpdateNewQuoteIdAsync(quoteNumber, newQuoteNumber);
        Log.Information("New quote id updated");
        return Ok();
    }

    [HttpPost("import-excel")]
    [Authorize(Roles = "Verkoper")]
    public async Task<IActionResult> ImportExcel([FromBody] FileUploadModel fileUpload)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        if (string.IsNullOrEmpty(fileUpload.FileBase64))
        {
            Log.Warning("No file data was uploaded.");
            return BadRequest("No file data was uploaded.");
        }

        var excelModel = await quoteService.ImportFromExcelAsync(fileUpload.FileBase64, userId);
        Log.Information("Excel file imported");
        return Ok(excelModel);
    }

    [HttpGet("total")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<int> GetTotal()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        if (User.IsInRole("Verkoper"))
        {
            Log.Information("Total quotes retrieved for salesperson");
            return await quoteService.GetTotalQuotesForSalespersonAsync(userId);
        }

        Log.Information("Total quotes retrieved for admin");
        return await quoteService.GetTotalQuotesAsync();
    }
    
    [HttpPost]
    [Authorize(Roles = "Verkoper")]
    public async Task<ActionResult<QuoteDto.Index>> Post(QuoteDto.Create quote)
    {
        var newQuote = await quoteService.CreateQuoteAsync(quote);
        Log.Information("Quote created");
        return CreatedAtAction(nameof(Get), new { id = newQuote.Id }, newQuote);
    }

    [HttpPatch("{quoteNumber}/approve")]
    [Authorize(Roles = "Verkoper")]
    public async Task<bool> ApproveQuote(string quoteNumber)
    {
        Log.Information("Quote approved");
        return await quoteService.ApproveQuote(quoteNumber);
    }
    /// <summary>
    /// Generates a PDF for the specified quote number.
    /// </summary>
    /// <param name="quoteNumber">The quote number.</param>
    /// <returns>The PDF file.</returns>
    [HttpGet("{quoteNumber}/pdf")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IActionResult> GetQuotePdf(string quoteNumber)
    {
        try
        {
            var pdfBytes = await quoteService.GeneratePdf(quoteNumber);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                Log.Warning("PDF generation failed: received empty byte array.");
                throw new Exception("PDF generation failed: received empty byte array.");
            }

            Log.Information("PDF generated");
            return File(pdfBytes, "application/pdf", $"{quoteNumber}.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Er is een fout opgetreden tijdens het genereren van de PDF: {ex.Message}");
        }
    }

    [HttpPut("{quoteNumber}")]
    [Authorize(Roles = "Verkoper")]
    public async Task<ActionResult<QuoteDto.Index>> Put(string quoteNumber, QuoteDto.Update quote)
    {
        var updatedQuote = await quoteService.UpdateQuoteAsync(quoteNumber, quote);
        Log.Information("Quote updated");
        return Ok(updatedQuote);
    }

}

public class FileUploadModel
{
    public string FileBase64 { get; set; } = string.Empty;
}
