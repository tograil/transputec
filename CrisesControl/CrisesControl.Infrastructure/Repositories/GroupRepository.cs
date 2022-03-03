using CrisesControl.Core.GroupAggregate;
using CrisesControl.Core.GroupAggregate.Repositories;
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
            await _context.AddAsync(group);

            await _context.SaveChangesAsync();

            return group.GroupId;
        }

        public async Task<int> DeleteGroup(int groupId, CancellationToken token)
        {
            await _context.AddAsync(groupId);

            await _context.SaveChangesAsync(token);

            return groupId;
        }

        public async Task<IEnumerable<Group>> GetAllGroups()
        {
            return await _context.Set<Group>().AsNoTracking().ToArrayAsync();
        }

        public async Task<Group> GetById(int groupId)
        {
            return await _context.Set<Group>().AsNoTracking().Where(t => t.GroupId == groupId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateGroup(Group group, CancellationToken token)
        {
            await _context.AddAsync(group, token);
            await _context.SaveChangesAsync(token);
            return group.GroupId;
        }

    }
}
