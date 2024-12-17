using Microsoft.AspNetCore.Mvc;
using Rise.Domain.Exceptions;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace Rise.Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MachineryController(IMachineryService machineryService) : ControllerBase
{

    private readonly IMachineryService machineryService = machineryService;

    [HttpGet]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<MachineryDto.Detail>> Get([FromQuery] MachineryQueryObject query)

    {
        var machineries = await machineryService.GetMachineriesAsync(query);
        Log.Information("Machineries retrieved");
        return machineries;
    }


    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<MachineryDto.Detail> Get(int id)
    {

        var machinery = await machineryService.GetMachineryAsync(id);
        Log.Information("Machinery retrieved by id");
        return machinery;

    }

    [HttpGet("total")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<int> GetTotal()
    {
        Log.Information("Total machineries retrieved");
        return await machineryService.GetTotalMachineriesAsync();
    }

    [HttpGet("detail/{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<MachineryDto.XtremeDetail> GetDetail(int id, [FromQuery] OptionQueryObject query)
    {
        var machinery = await machineryService.GetMachineryAsyncWithCategories(id, query);
        Log.Information("Machinery retrieved with categories");
        return machinery;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<MachineryResult.Create> Post(MachineryDto.Create machinery)
    {
        Log.Information("Machinery created");
        return await machineryService.CreateMachineryAsync(machinery);
    }

    [HttpPut]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<MachineryResult.Create>> Put(MachineryDto.Update machinery)
    {
        var updatedMachinery = await machineryService.UpdateMachineryAsync(machinery.Id, machinery);
        Log.Information("Machinery updated");
        return updatedMachinery;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> Delete(int id)
    {
        await machineryService.DeleteMachineryAsync(id);
        Log.Information("Machinery deleted");
        return NoContent();
    }

    [HttpDelete("{id}/{imageId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> DeleteImageMachineryAsync(int id, int imageId)
    {
        await machineryService.DeleteImageMachineryAsync(id, imageId);
        Log.Information("Image deleted");
        return NoContent();
    }

}




