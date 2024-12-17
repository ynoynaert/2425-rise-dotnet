using Blazorise;
using Rise.Shared.Locations;
using Rise.Shared.Users;
using System.Net.Http.Json;

namespace Rise.Client.Locations;

public class LocationService(HttpClient httpClient) : ILocationService
{
    public Task<LocationDto.Index> GetLocationAsync(int id)
    {
        var location = httpClient.GetFromJsonAsync<LocationDto.Index>($"location/{id}");
        return location!;
    }

    public async Task<IEnumerable<LocationDto.Index>> GetLocationsAsync()
    {
        var locations = await httpClient.GetFromJsonAsync<IEnumerable<LocationDto.Index>>("location");
        return locations!;
    }

    public async Task<IEnumerable<LocationDto.Detail>> GetLocationsWithSalesPeopleAsync()
    {
        var locations = await httpClient.GetFromJsonAsync<IEnumerable<LocationDto.Detail>>($"location/users");
        return locations!;
    }
}
