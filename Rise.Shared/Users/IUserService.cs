using Rise.Shared.Helpers;

namespace Rise.Shared.Users;

public interface IUserService
{
    Task<IEnumerable<UserDto.Index>> GetUsersAsync();
    Task<IEnumerable<UserDto.Index>> GetSalesPeopleAsync(UserQueryObject query);
    Task<UserDto.Index> CreateUserAsync(UserDto.Create userDto);
    Task<int> GetTotalSalesPeopleAsync();
    Task<UserDto.Index> GetSalesPersonAsync(string SalespersonId);
}
