using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Helpers;
using Rise.Shared.Users;
using Serilog;

namespace Rise.Server.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]

public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService userService = userService;

    [HttpGet]
    public async Task<IEnumerable<UserDto.Index>> GetUsers()
    {
        Log.Information("Users retrieved");
        return await userService.GetUsersAsync();
    }

    [HttpGet("verkopers")]
    public async Task<IEnumerable<UserDto.Index>> GetVerkopers([FromQuery] UserQueryObject query)
    {
        Log.Information("Salespeople retrieved");
        return await userService.GetSalesPeopleAsync(query);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto.Index>> Post(UserDto.Create userDto)
    {
        Log.Information("User created");
        return await userService.CreateUserAsync(userDto);
    }

    [HttpGet("total")]
    public async Task<int> GetTotal()
    {
        Log.Information("Total users retrieved");
        return await userService.GetTotalSalesPeopleAsync();
    }
    
}