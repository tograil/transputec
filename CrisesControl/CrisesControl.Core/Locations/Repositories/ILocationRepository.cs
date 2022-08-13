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
        Task<bool> DeleteLocation(int LocationId, int CompanyId, int UserId, string TimeZoneId = "GMT Standard Time");
        Task<List<GroupLink>> SegregationLinks(int TargetID, string MemberShipType, string LinkType, int CurrentUserId, int OutUserCompanyId);
        Task<bool> UpdateSegregationLink(int SourceID, int TargetID, string Action, GroupType LinkType, int CompanyId);
    }
}
