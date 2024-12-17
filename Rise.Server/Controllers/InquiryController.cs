using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Domain.Exceptions;
using Rise.Shared.Inquiries;

namespace Rise.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InquiryController(IInquiryService inquiryService) : ControllerBase
{
    private readonly IInquiryService inquiryService = inquiryService;

    [HttpPost]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<ActionResult<InquiryDto.Index>> Post(InquiryDto.Create inquiryDto)
    {
            var inquiry = await inquiryService.CreateInquiryAsync(inquiryDto);
            return Ok(inquiry);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<ActionResult> Delete(int id)
    {
        await inquiryService.DeleteInquiryAsync(id);
        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<InquiryDto.Index>> Get()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        if (User.IsInRole("Verkoper"))
        {
            return await inquiryService.GetInquiriesForSalespersonAsync(userId);
        }
        return await inquiryService.GetInquiriesAsync();
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<ActionResult<InquiryDto.Index>> Get(int id)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        if (User.IsInRole("Verkoper"))
        {
            return Ok(await inquiryService.GetInquiryForSalespersonAsync(id, userId));
        }
        var inquiry = await inquiryService.GetInquiryAsync(id);
        return Ok(inquiry);
    }

}
