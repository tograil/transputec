using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class UserRelations
    {
        public bool ShowAllGroups { get; set; }
        public List<UGroup> Groups { get; set; }
        public List<int> Users { get; set; }
    }
}
