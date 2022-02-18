using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupModel = CrisesControl.Core.Models.Group;

namespace CrisesControl.Core.GroupAggregate.Services
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupModel>> GetAllGroups();
    }
}
