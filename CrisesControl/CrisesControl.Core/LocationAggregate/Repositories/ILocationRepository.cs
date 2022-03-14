using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.LocationAggregate.Services
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAllLocations(int companyId);
        Task<Location> GetLocationById(int locationId);
        Task<int> CreateLocation(Location location, CancellationToken cancellationToken);
        Task<int> UpdateLocation(Location location, CancellationToken cancellationToken);
        Task<int> DeleteLocation(int locationId, CancellationToken cancellationToken);
    }
}
