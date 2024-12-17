using Rise.Shared.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Locations;

public interface ILocationService
{
    Task<IEnumerable<LocationDto.Index>> GetLocationsAsync();
    Task<LocationDto.Index> GetLocationAsync(int id);
    Task<IEnumerable<LocationDto.Detail>> GetLocationsWithSalesPeopleAsync();
}
