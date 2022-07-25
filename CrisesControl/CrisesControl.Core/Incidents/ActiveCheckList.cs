using CrisesControl.Core.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class ActiveCheckList
    {
        public int ActiveCheckListID { get; set; }
        public int CheckListID { get; set; }
        public int ActiveTaskID { get; set; }
        public string Description { get; set; }
        public bool DoneOnly { get; set; }
        public int OptionCount { get; set; }
        public int SortOrder { get; set; }
        public List<CheckListOption> CheckListOptions { get; set; }
        public List<UsrResponse> UserResponse { get; set; }
    }
}
