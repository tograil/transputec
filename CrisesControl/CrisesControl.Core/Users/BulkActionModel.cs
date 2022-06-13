using CrisesControl.Core.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class BulkActionModel: CcBase
    {
        public string Action { get; set; }
        public List<int> UserList { get; set; }
        public string UserRole { get; set; }
        public string SetUserRole { get; set; }
        public string SecurityGroups { get; set; }
        public string SetSecurityGroups { get; set; }
        public string Filters { get; set; }
        public string GroupActionLocation { get; set; }
        public string GroupActionGroup { get; set; }
        public string SetLocation { get; set; }
        public string SetGroup { get; set; }
        public string ObjMapId { get; set; }
        public int Status { get; set; }
        public string SetStatus { get; set; }
        public bool ExpirePassword { get; set; }
        public int[] PingMethod { get; set; }
        public string SetPingMethod { get; set; }
        public int[] IncidentMethod { get; set; }
        public string SetIncidentMethod { get; set; }
        public string UserLanguage { get; set; }
        public string SetUserLanguage { get; set; }
        public bool SendInvite { get; set; }
        public int Department { get; set; }
        public string SetDepartment { get; set; }
    }
}
