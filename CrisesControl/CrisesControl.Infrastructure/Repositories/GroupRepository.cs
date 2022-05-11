using CrisesControl.Core.Groups;
using CrisesControl.Core.Groups.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class GroupRepository: IGroupRepository
    {
        private readonly CrisesControlContext _context;

        public GroupRepository(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<int> CreateGroup(Group group, CancellationToken token)
        {
            await _context.AddAsync(group, token);

            await _context.SaveChangesAsync(token);

            return group.GroupId;
        }

        public async Task<int> DeleteGroup(int groupId, CancellationToken token)
        {
            await _context.AddAsync(groupId);

            await _context.SaveChangesAsync(token);

            return groupId;
        }

        public async Task<IEnumerable<Group>> GetAllGroups(int companyId)
        {
            return await _context.Set<Group>().AsNoTracking().Where(t => t.CompanyId == companyId).ToListAsync();
        }


        public async Task<Group> GetGroup(int companyId, int groupId)
        {
            return await _context.Set<Group>().AsNoTracking().Where(t => t.CompanyId == companyId && t.GroupId == groupId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateGroup(Group group, CancellationToken token)
        {
            var result = _context.Set<Group>().Where(t => t.GroupId == group.GroupId).FirstOrDefault();

            if (result == null)
            {
                return default;
            }
            else
            {
                result.GroupName = group.GroupName;
                result.Status = group.Status;
                result.UpdatedOn = group.UpdatedOn;
                result.UpdatedBy = group.UpdatedBy;
                await _context.SaveChangesAsync(token);
                return result.GroupId;
            }
        }

        public bool CheckDuplicate(Group group)
        {
            return _context.Set<Group>().Where(t=>t.GroupName.Equals(group.GroupName)).Any();
        }

        public bool CheckForExistance(int groupId)
        {
            return _context.Set<Group>().Where(t=>t.GroupId.Equals(groupId)).Any();
        }
    }
}
