using Rise.Persistence;
using Rise.Shared.Locations;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Shared.Users;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Serilog;
using Rise.Services.Users;
using Rise.Shared.Helpers;

namespace Rise.Services.Locations;

public class LocationService(ApplicationDbContext dbContext, IManagementApiClient managementApiClient) : ILocationService
{
    private readonly ApplicationDbContext dbContext = dbContext;
    public readonly IManagementApiClient managementApiClient = managementApiClient;
    public async Task<LocationDto.Index> GetLocationAsync(int id)
    {
        var location = await dbContext.Location
            .Where(x => !x.IsDeleted)
            .Select(x => new LocationDto.Index
            {
                Id = x.Id,
                Name = x.Name,
                Street = x.Street,
                StreetNumber = x.StreetNumber,
                City = x.City,
                PostalCode = x.PostalCode,
                Country = x.Country,
                Image = x.Image,
                PhoneNumber = x.PhoneNumber,
                VatNumber = x.VatNumber,
                Code = x.Code
            }).SingleOrDefaultAsync(x => x.Id == id);

        Log.Information("Location retrieved by id");
        return location ?? throw new EntityNotFoundException("Vestiging", id);
    }

    public async Task<IEnumerable<LocationDto.Index>> GetLocationsAsync()
    {
        IQueryable<LocationDto.Index> query = dbContext.Location
            .Where(x => !x.IsDeleted)
            .Select(x => new LocationDto.Index
            {
                Id = x.Id,
                Name = x.Name,
                Street = x.Street,
                StreetNumber = x.StreetNumber,
                City = x.City,
                PostalCode = x.PostalCode,
                Country = x.Country,
                Image = x.Image,
                PhoneNumber = x.PhoneNumber,
                VatNumber = x.VatNumber,
                Code = x.Code
            });
        var locations = await query.ToListAsync();

        Log.Information("Locations retrieved");
        return locations;
    }

    public async Task<IEnumerable<LocationDto.Detail>> GetLocationsWithSalesPeopleAsync()
    {
        var locations = await GetLocationsAsync();
        var salesPeople = await GetSalesPeopleAsync();

        var salesPeopleByLocationCode = salesPeople
            .GroupBy(sp => sp.Location!.Code)
            .ToDictionary(g => g.Key, g => g.ToList());

        var locationDetails = locations.Select(location =>
        {
            salesPeopleByLocationCode.TryGetValue(location.Code, out var matchingSalesPeople);

            return new LocationDto.Detail
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
                SalesPeople = matchingSalesPeople ?? []
            };
        });

        Log.Information("Locations with salespeople retrieved");
        return locationDetails;
    }

    public async Task<IEnumerable<UserDto.Index>> GetSalesPeopleAsync()
    {
        var users = await managementApiClient.Users.GetAllAsync(new GetUsersRequest
        {
            Sort = "name:1"
        });

        var verkopers = new List<UserDto.Index>();

        var roles = await managementApiClient.Roles.GetAllAsync(new GetRolesRequest());
        var role = roles.FirstOrDefault(x => x.Name == "Verkoper") ?? throw new EntityNotFoundException("Rol", "Verkoper");

        foreach (var user in users)
        {
            if (user.AppMetadata["role"] == role.Name)
            {
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

        Log.Information("Salespeople retrieved");
        return verkopers.ToList() ?? [];
    }
}
