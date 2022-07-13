using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class AdminLibIncident
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int LibIncidentTypeId { get; set; }
        public string LibIncidentIcon { get; set; }
        public int Severity { get; set; }
        public int Status { get; set; }
        public int IsDefault { get; set; }
        public string IncidentTypeName { get; set; }
    }
}
