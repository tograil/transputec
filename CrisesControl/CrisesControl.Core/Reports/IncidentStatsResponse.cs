using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class IncidentStatsResponse
    {
        public int IncidentActivationId { get; set; }
        public int TotalMessageSent { get; set; }
        public int UserNotified { get; set; }
        public int TotalACK { get; set; }
        public int TotalUnAck { get; set; }
        [NotMapped]
        public IncidentStat IncidentKPI { get; set; }
        public int Email { get; set; }
        public int Push { get; set; }
        public int Text { get; set; }
        public int Phone { get; set; }
    }
}
