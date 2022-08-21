using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Companies
{
    public class CompanyScim
    {
        public string ScimToken { get; set; }
        public int TokenExpiry { get; set; }
        public string DefaultMenuAccess { get; set; }
        public string DefaultGroup { get; set; }
        public string DefaultLocation { get; set; }
        public string DefaultDepartment { get; set; }
        public string DefaultMobile { get; set; }
        public bool TokenGenerated { get; set; }
        public string NotificationEmails { get; set; }
        public bool SendInvitation { get; set; }
        public string PingMethods { get; set; }
        public string IncidentMethods { get; set; }
    }
}
