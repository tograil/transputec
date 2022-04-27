using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CompanyParameters
{
    public class CascadingPlanReturn
    {
        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public string PlanType { get; set; }
        public bool LaunchSOS { get; set; }
        public int LaunchSOSInterval { get; set; }
        public List<CommsMethodPriority> CommsMethod { get; set; }
    }
}
