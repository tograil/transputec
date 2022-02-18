using CrisesControl.Core.GroupAggregate.Services;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class GroupService: IGroupService
    {
        private readonly CrisesControlContext _context;

        public GroupService(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Group>> GetAllGroups()
        {
            return await _context.Set<Group>().AsNoTracking().ToArrayAsync();
        }

    }
}
