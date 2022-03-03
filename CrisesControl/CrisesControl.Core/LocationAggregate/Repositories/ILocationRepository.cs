using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Core.LocationAggregate.Services
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAllLocations();
        Task<Location> GetLocationById(int locationId);
        Task<Location> CreateLocation(Location location);
        Task<Location> UpdateLocation(Location location);
        Task DeleteLocation(int locationId);
    }
}
