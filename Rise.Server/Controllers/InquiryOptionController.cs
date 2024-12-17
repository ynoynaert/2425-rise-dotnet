using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Inquiries;

namespace Rise.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InquiryOptionController(IInquiryOptionService inquiryOptionService) : ControllerBase
{
    private readonly IInquiryOptionService inquiryOptionService = inquiryOptionService;

    [HttpGet]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<InquiryOptionDto.Index>> Get()
    {
        var inquiryOptions = await inquiryOptionService.GetInquiryOptionsAsync();
        return inquiryOptions;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<InquiryOptionDto.Index> Get(int id)
    {
        var inquiryOption = await inquiryOptionService.GetInquiryOptionAsync(id);
        return inquiryOption;
    }

    [HttpGet("ByInquiry/{inquiryId}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<InquiryOptionDto.Index>> GetByInquiryId(int inquiryId)
    {
        var inquiryOptions = await inquiryOptionService.GetInquiryOptionsByInquiryIdAsync(inquiryId);
        return inquiryOptions;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<ActionResult> Delete(int id)
    {
        await inquiryOptionService.DeleteInquiryOptionAsync(id);
        return NoContent();
    }

    [HttpPost]
    [Authorize (Roles = "Administrator, Verkoper")]
    public async Task<ActionResult<InquiryOptionDto.Index>> Post(InquiryOptionDto.Create inquiryOption)
    {
        return await inquiryOptionService.CreateInquiryOptionAsync(inquiryOption);
    }
}
