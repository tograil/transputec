using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class GroupOnlyImportModel
    {
        public string SessionId { get; set; }
        public List<GroupData> DeptData { get; set; }

    }

    public class GroupData
    {
        public GroupData()
        {
            Action = "ADD";
            Status = 0;
        }
        public string GroupName { get; set; }
        public int Status { get; set; }
        public string Action { get; set; }
    }
}
