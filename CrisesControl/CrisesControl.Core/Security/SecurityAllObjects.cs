using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Security
{
    public class SecurityAllObjects
    {
        public int SecurityObjectID { get; set; }
        public int ParentID { get; set; }
        public int TypeID { get; set; }
        public string ObjectType { get; set; }
        public string SecurityKey { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string Target { get; set; }
        public bool RequireKeyHolder { get; set; }
        public int RoleID { get; set; }
        public bool RequireAdmin { get; set; }
        public decimal MenuOrder { get; set; }
        public int ForIncidentManager { get; set; }
      
    }
}
