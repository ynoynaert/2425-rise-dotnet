using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Machineries;
using Serilog;
using System.Security.Claims;

namespace Rise.Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MachineryOptionController(IMachineryOptionService machineryOptionService) : ControllerBase
{
	private readonly IMachineryOptionService machineryOptionService = machineryOptionService;

    [HttpGet]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<MachineryOptionDto.Detail>> Get()
	{
		var machineryOption = await machineryOptionService.GetMachineryOptionsAsync();
        Log.Information("Machinery options retrieved");
        return machineryOption;
	}

	[HttpGet("{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<MachineryOptionDto.Detail> Get(int id)
    {
        var machineryOption = await machineryOptionService.GetMachineryOptionAsync(id);
        Log.Information("Machinery option retrieved by id");
        return machineryOption;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<MachineryOptionDto.Detail>> Post(MachineryOptionDto.Create machineryOption)
    {
        var newMachineryOption = await machineryOptionService.CreateMachineryOptionAsync(machineryOption);
        Log.Information("Machinery option created");
        return CreatedAtAction(nameof(Get), new { id = newMachineryOption.Id }, newMachineryOption);
    }

    [HttpPut]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<MachineryOptionDto.Detail>> Put(MachineryOptionDto.Update machineryOption)
    {
        var updatedMachineryOption = await machineryOptionService.UpdateMachineryOptionAsync(machineryOption.Id, machineryOption);
        Log.Information("Machinery option updated");
        return updatedMachineryOption;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> Delete(int id)
    {
        await machineryOptionService.DeleteMachineryOptionAsync(id);
        Log.Information("Machinery option deleted");
        return NoContent();
    }

    [HttpPost("import")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> ImportExcel([FromBody] FileUploadModel fileUpload)
    {
        if (string.IsNullOrEmpty(fileUpload.FileBase64))
        {
            Log.Warning("No file data was uploaded.");
            return BadRequest("No file data was uploaded.");
        }

        var result = await machineryOptionService.ImportPriceUpdateFile(fileUpload.FileBase64);
        Log.Information("Machinery option price updated");
        return Ok(result);
    }

}




