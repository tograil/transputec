using CrisesControl.Core.Groups;
using CrisesControl.SharedKernel.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Locations.Services
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAllLocations(int companyId);
        Task<Location> GetLocationById(int locationId);
        Task<int> CreateLocation(Location location, CancellationToken cancellationToken);
        Task<int> UpdateLocation(Location location, CancellationToken cancellationToken);
        Task<int> DeleteLocation(int locationId, CancellationToken cancellationToken);
        bool CheckDuplicate(Location location);
        bool CheckForExisting(int locationId);
        Task<bool> DeleteLocation(int locationId, int companyId, int userId, string timeZoneId = "GMT Standard Time");
        Task<List<GroupLink>> SegregationLinks(int targetID, string memberShipType, string linkType, int currentUserId, int outUserCompanyId);
        Task<bool> UpdateSegregationLink(int sourceID, int targetID, GroupType linkType, int companyId);
    }
}
