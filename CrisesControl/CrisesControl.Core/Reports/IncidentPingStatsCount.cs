
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Reports {
    public class IncidentPingStatsCount {
        public int TotalPingInKPI { get; set; }
        [NotMapped]
        public int TotalPingOutKPI { get; set; }
        public int TotalPingOutKPIMax { get; set; }
        public int TotalPingACK { get; set; }
        public int TotalPingSent { get; set; }
        public int TotalIncidentInKPI { get; set; }
        [NotMapped]
        public int TotalIncidentOutKPI { get; set; }
        public int TotalIncidentOutKPIMax { get; set; }
        public int TotalIncidentACK { get; set; }
        public int TotalIncidentSent { get; set; }
        [NotMapped]
        public int TotalSent { get; set; }
        public long TotalCreated { get; set; }
        public double PingKPI { get; set; }
        public double PingMaxKPI { get; set; }
        public double IncidentKPI { get; set; }
        public double IncidentMaxKPI { get; set; }
        public int TotalPingOneHour { get; set; }
        public int TotalPingWithInCutOff { get; set; }
        public int TotalPingNoResponse { get; set; }
        public int TotalIncidentOneHour { get; set; }
        public int TotalIncidentWithInCutOff { get; set; }
        public int TotalIncidentNoResponse { get; set; }
        public string MonthName { get; set; }
        public int YearName { get; set; }
        public int App { get; set; }
        public int Email { get; set; }
        public int Text { get; set; }
        public int Phone { get; set; }
        public int Web { get; set; }
    }
}
