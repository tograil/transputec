using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class LibIncident
    {

        public int LibIncidentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int LibIncidentTypeId { get; set; }
        public string LibIncodentIcon { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTimeOffset UpdatedOn { get; set; }
        public int Severity { get; set; }
        public int IsDefault { get; set; }
        public bool IsSOS { get; set; }
    }
}
