using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Blazorise;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Persistence;
using Rise.Shared.Helpers;
using Rise.Shared.Locations;
using Rise.Shared.Users;
using Serilog;

namespace Rise.Services.Users;

public class UserService(ApplicationDbContext dbContext, IManagementApiClient managementApiClient) : IUserService
{
    public async Task<IEnumerable<UserDto.Index>> GetUsersAsync()
    {
        var users = await managementApiClient.Users.GetAllAsync(new GetUsersRequest());

        Log.Information("Users retrieved");
        return users.Select(x => new UserDto.Index
        {
            Email = x.Email,
            Name = x.FullName,
            PhoneNumber = x.PhoneNumber,
        }) ?? [];
    }

    public async Task<IEnumerable<UserDto.Index>> GetSalesPeopleAsync(UserQueryObject query)
    {
        var users = await managementApiClient.Users.GetAllAsync(new GetUsersRequest());

        var verkopers = new List<UserDto.Index>();

		var roles = await managementApiClient.Roles.GetAllAsync(new GetRolesRequest());
		var role = roles.FirstOrDefault(x => x.Name == "Verkoper") ?? throw new EntityNotFoundException("Rol", "Verkoper");

        foreach (var user in users) {
            if (user.AppMetadata["role"] == role.Name)
            {
                Log.Information("User is a salesperson");
                var code = (string)user.AppMetadata["location"];
                var location = await dbContext.Location.Where(x => x.Code == code).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Vestiging", code);

                verkopers.Add(new UserDto.Index
                {
                    Email = user.Email,
                    Name = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Location = new LocationDto.Index
                    {
                        Id = location.Id,
                        Name = location.Name,
                        Code = location.Code,
                        VatNumber = location.VatNumber,
                        Street = location.Street,
                        StreetNumber = location.StreetNumber,
                        City = location.City,
                        PostalCode = location.PostalCode,
                        Country = location.Country,
                        Image = location.Image,
                        PhoneNumber = location.PhoneNumber,
                    },
                });
            }
		}

        verkopers = verkopers.Where(x =>
                                (query.Search == null ||
                                 x.Name.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                                 x.Email.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                                 x.PhoneNumber.Contains(query.Search, StringComparison.OrdinalIgnoreCase)) &&
                                (query.LocationIds == null || query.LocationIds.Contains(x.Location!.Id.ToString())))
                             .OrderBy(x => x.Name)
                             .Skip((query.PageNumber - 1) * query.PageSize)
                             .Take(query.PageSize)
                             .ToList();

        Log.Information("Salespeople retrieved");
        return verkopers ?? [];
    }

    public async Task<UserDto.Index> CreateUserAsync(UserDto.Create userDto)
    {
        var location = await dbContext.Location.Where(x => x.Id == userDto.LocationId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Vestiging", userDto.LocationId);
		var roles = await managementApiClient.Roles.GetAllAsync(new GetRolesRequest());
        var role = roles.FirstOrDefault(x => x.Name == "Verkoper") ?? throw new EntityNotFoundException("Rol", "Verkoper");
        var createRequest = new UserCreateRequest
        {
            Email = userDto.Email,
            Password = userDto.Password,
            FullName = userDto.Name,
			PhoneNumber = userDto.PhoneNumber,
			AppMetadata = new Dictionary<string, object>
			{
				["role"] = role.Name,
                ["location"] = location.Code,
			},
			Connection = "Username-Password-Authentication",
        };

        var createdUser = await managementApiClient.Users.CreateAsync(createRequest);

		try
        {
            Log.Information("Role being assinged to user");
            var rolesRequest = new AssignRolesRequest
            {
                Roles = [role.Id],
            };

            Log.Information("Role assigned to user");
            await managementApiClient.Users.AssignRolesAsync(createdUser.UserId, rolesRequest);
        }
		catch (Exception ex)
		{
			await managementApiClient.Users.DeleteAsync(createdUser.UserId);
			throw new Exception("Er is een fout opgetreden bij het toekennen van de rol 'Verkoper': " + ex.Message);
		}

        Log.Information("User created");
        return new UserDto.Index
        {
            Email = createdUser.Email,
            Name = createdUser.FullName,
            PhoneNumber = createdUser.PhoneNumber,
        };
    }

    public async Task<int> GetTotalSalesPeopleAsync()
    {
        var users = await managementApiClient.Users.GetAllAsync(new GetUsersRequest());

        int total = 0;

        var roles = await managementApiClient.Roles.GetAllAsync(new GetRolesRequest());
        var role = roles.FirstOrDefault(x => x.Name == "Verkoper") ?? throw new EntityNotFoundException("Rol", "Verkoper");

        foreach (var user in users)
        {
            if (user.AppMetadata["role"] == role.Name)
            {
                total++;
            }
        }

        Log.Information("Total salespeople retrieved");
        return total;
    }

    public async Task<UserDto.Index> GetSalesPersonAsync(string SalespersonId)
    {
        var user = await managementApiClient.Users.GetAsync(SalespersonId);

        if (user == null)
        {
            Log.Warning("User not found");
            throw new EntityNotFoundException("Gebruiker", SalespersonId);
        }

        var roles = await managementApiClient.Roles.GetAllAsync(new GetRolesRequest());
        var role = roles.FirstOrDefault(x => x.Name == "Verkoper") ?? throw new EntityNotFoundException("Rol", "Verkoper");

        if (user.AppMetadata["role"] != role.Name)
        {
            Log.Warning("User is not a salesperson");
            throw new EntityNotFoundException("Gebruiker", SalespersonId);
        }

        var code = (string)user.AppMetadata["location"];
        var location = await dbContext.Location.Where(x => x.Code == code).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Vestiging", code);

        Log.Information("Salesperson retrieved");
        return new UserDto.Index
        {
            Email = user.Email,
            Name = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Location = new LocationDto.Index
            {
                Id = location.Id,
                Name = location.Name,
                Code = location.Code,
                VatNumber = location.VatNumber,
                Street = location.Street,
                StreetNumber = location.StreetNumber,
                City = location.City,
                PostalCode = location.PostalCode,
                Country = location.Country,
                Image = location.Image,
                PhoneNumber = location.PhoneNumber,
            },
        };
    }
}
