using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.GroupAggregate.Repositories
{
    public interface IGroupRepository
    {
        Task<IEnumerable<Group>> GetAllGroups();
        Task<Group> GetById(int groupId);
        Task<int> CreateGroup(Group payload, CancellationToken token);
        Task<int> UpdateGroup(Group payload, CancellationToken token);
        Task<int> DeleteGroup(int groupId, CancellationToken token);
    }
}
