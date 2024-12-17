using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Quotes;
using Serilog;

namespace Rise.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradedMachineryController(ITradedMachineryService tradedMachineryService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Verkoper")]
    public async Task<ActionResult<TradedMachineryResult.Create>> Post(TradedMachineryDto.Create tradedMachinery)
    {
        Log.Information("Creating traded machinery");
        return await tradedMachineryService.CreateTradedMachineryAsync(tradedMachinery);
    }
}
