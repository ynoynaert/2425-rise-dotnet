using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Machineries;
using Serilog;

namespace Rise.Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class OptionController(IOptionService optionService) : ControllerBase
{
	private readonly IOptionService optionService = optionService;

    [HttpGet]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<OptionDto.Detail>> Get()
	{
		var options = await optionService.GetOptionsAsync();
        Log.Information("Options retrieved");
        return options;
	}

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<OptionDto.Detail> Get(int id)
    {
        var option = await optionService.GetOptionAsync(id);
        Log.Information("Option retrieved by id");
        return option;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<OptionDto.Detail>> Post(OptionDto.Create option)
    {
        var newOption = await optionService.CreateOptionAsync(option);
        Log.Information("Option created");
        return CreatedAtAction(nameof(Get), new { id = newOption.Id }, newOption);
    }

    [HttpPut]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<OptionDto.Detail>> Put(OptionDto.Update option)
    {
        var updatedOption = await optionService.UpdateOptionAsync(option.Id, option);
        Log.Information("Option updated");
        return updatedOption;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> Delete(int id)
    {
        await optionService.DeleteOptionAsync(id);
        Log.Information("Option deleted");
        return NoContent();
    }
}




