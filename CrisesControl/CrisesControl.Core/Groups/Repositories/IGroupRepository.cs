using CrisesControl.SharedKernel.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Groups.Repositories
{
    public interface IGroupRepository
    {
        Task<IEnumerable<Group>> GetAllGroups(int companyId);
        Task<Group> GetGroup(int companyId, int groupId);
        Task<int> CreateGroup(Group payload, CancellationToken token);
        Task<int> UpdateGroup(Group payload, CancellationToken token);
        Task<int> DeleteGroup(int groupId, CancellationToken token);
        bool CheckDuplicate(Group group);
        bool CheckForExistance(int groupId);
        Task<List<GroupLink>> SegregationLinks(int TargetID, MemberShipType MemberShipType, string LinkType);
    }
}
