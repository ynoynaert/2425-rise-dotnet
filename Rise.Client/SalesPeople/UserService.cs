using Rise.Shared.Helpers;
using Rise.Shared.Users;
using System.Net.Http.Json;

namespace Rise.Client.SalesPeople;

public class UserService(HttpClient httpClient) : IUserService
{
    public async Task<IEnumerable<UserDto.Index>> GetUsersAsync()
    {
        var result = await httpClient.GetFromJsonAsync<IEnumerable<UserDto.Index>>("user");
        return result ?? Enumerable.Empty<UserDto.Index>();
    }

    public async Task<IEnumerable<UserDto.Index>> GetSalesPeopleAsync(UserQueryObject spqs)
    {
        var result = await httpClient.GetFromJsonAsync<IEnumerable<UserDto.Index>>($"user/verkopers?Search={spqs.Search}&LocationIds={spqs.LocationIds}&PageNumber={spqs.PageNumber}");
        return result ?? Enumerable.Empty<UserDto.Index>();
    }

    public async Task<UserDto.Index> CreateUserAsync(UserDto.Create userDto)
    {
        var result = await httpClient.PostAsJsonAsync<UserDto.Create>("user", userDto);
        var user = await result.Content.ReadFromJsonAsync<UserDto.Index>();
        return user!;
    }

    public async Task<int> GetTotalSalesPeopleAsync()
    {
        return await httpClient.GetFromJsonAsync<int>("user/total");
    }

    public Task<UserDto.Index> GetSalesPersonAsync(string SalespersonId)
    {
        throw new NotImplementedException();
    }
}
