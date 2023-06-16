using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class IncidentLibraryDetails
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int LibIncidentId { get; set; }
        public string IncidentTypeName { get; set; }
        public string IncidentIcon { get; set; }
        public int Severity { get; set; }
    }
}
