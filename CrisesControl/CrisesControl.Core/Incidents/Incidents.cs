using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class Incidents
    {
        public string? Name { get; set; }
        public string? IncidentIcon { get; set; }
        public DateTimeOffset LaunchedOn { get; set; }
        public decimal? RTO { get; set; }
        public decimal? RPO { get; set; }
    }
}
