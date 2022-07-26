using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class IncidentSOSRequest
    {
        public int? SosalertId { get; set; }
        public int? UserId { get; set; }
        public int? ActiveIncidentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryEmail { get; set; }
        public string Isdcode { get; set; }
        public string MobileNo { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int ResponseLabel { get; set; }
        public DateTimeOffset ResponseTime { get; set; }
        public bool Completed { get; set; }
    }
}
