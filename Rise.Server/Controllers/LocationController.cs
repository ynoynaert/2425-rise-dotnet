using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Locations;
using Serilog;

namespace Rise.Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class LocationController(ILocationService locationService) : ControllerBase
{
    private readonly ILocationService locationService = locationService;

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IEnumerable<LocationDto.Index>> Get()
    {
        var locations = await locationService.GetLocationsAsync();
        Log.Information("Locations retrieved");
        return locations;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<LocationDto.Index> Get(int id)
    {
        var location = await locationService.GetLocationAsync(id);
        Log.Information("Location retrieved by id");
        return location;
    }

    [HttpGet("users")]
    [Authorize(Roles = "Administrator")]
    public async Task<IEnumerable<LocationDto.Detail>> GetLocationsWithSalesPeople()
    {
        var location = await locationService.GetLocationsWithSalesPeopleAsync();
        Log.Information("Locations with salespeople retrieved");
        return location;
    }
}
