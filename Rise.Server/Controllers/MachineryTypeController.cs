using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Machineries;
using Serilog;

namespace Rise.Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MachineryTypeController(IMachineryTypeService machineryTypeService) : ControllerBase
{
    private readonly IMachineryTypeService machineryTypeService = machineryTypeService;

    [HttpGet]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<MachineryTypeDto.Index>> Get()
    {
        var machineryTypes = await machineryTypeService.GetMachineryTypesAsync();
        Log.Information("Machinery types retrieved");
        return machineryTypes;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<MachineryTypeDto.Index> Get(int id)
    {
        var machineryType = await machineryTypeService.GetMachineryTypeAsync(id);
        Log.Information("Machinery type retrieved by id");
        return machineryType;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<MachineryTypeDto.Index>> Post(MachineryTypeDto.Create machineryType)
    {
        var newMachineryType = await machineryTypeService.CreateMachineryTypeAsync(machineryType);
        Log.Information("Machinery type created");
        return CreatedAtAction(nameof(Get), new { id = newMachineryType.Id }, newMachineryType);
    }

    [HttpPut]
    [Authorize(Roles = "Administrator")]
    public async Task<MachineryTypeDto.Index> Put(MachineryTypeDto.Update machineryType)
    {
        var updatedMachineryType = await machineryTypeService.UpdateMachineryTypeAsync(machineryType.Id, machineryType);
        Log.Information("Machinery type updated");
        return updatedMachineryType;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> DeleteMachineryTypeAsync(int id)
    {
        await machineryTypeService.DeleteMachineryTypeAsync(id);
        Log.Information("Machinery type deleted");
        return NoContent();
    }
}
