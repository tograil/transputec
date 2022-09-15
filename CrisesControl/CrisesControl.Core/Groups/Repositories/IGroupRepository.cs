using CrisesControl.SharedKernel.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Groups.Repositories
{
    public interface IGroupRepository
    {
        Task<List<GroupDetail>> GetAllGroups(int companyId, int userId = 0, int incidentId = 0 , bool filterVirtual = false);
        Task<GroupDetail> GetGroup(int companyId, int groupId);
        Task<int> CreateGroup(Group payload, CancellationToken token);
        Task<int> UpdateGroup(Group payload, CancellationToken token);
        Task<int> DeleteGroup(int groupId, CancellationToken token);
        bool CheckDuplicate(Group group);
        bool CheckForExistance(int groupId);
        Task<List<GroupLink>> SegregationLinks(int targetId, MemberShipType memberShipType, string linkType);
        Task<bool> DuplicateGroup(string strGroupName, int intcompanyid, int intGroupId, string strMode);
        Task<bool> UpdateSegregationLink(int sourceId, int targetId, string action, string linkType);
    }
}
