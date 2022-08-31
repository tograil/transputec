using CrisesControl.Core.Incidents;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Support
{
    public class IncidentsData
    {
        public int IncidentId { get; set; }
        public int CompanyId { get; set; }
        public bool TrackUser { get; set; }
        public int IncidentActivationId { get; set; }
        public string IncidentIcon { get; set; }
        public int IncidentSeverity { get; set; }
        public string IncidentPlan { get; set; }
        public string IncidentName { get; set; }
        public string IncidentLocation { get; set; }
        public UserFullName ActivatedBy { get; set; }
        public DateTimeOffset ActivatedOn { get; set; }
        public UserFullName LaunchedBy { get; set; }
        public DateTimeOffset LaunchedOn { get; set; }
        public UserFullName DeactivatedBy { get; set; }
        public DateTimeOffset DeactivatedOn { get; set; }
        public UserFullName ClosedBy { get; set; }
        public DateTimeOffset ClosedOn { get; set; }
        public int CurrentStatus { get; set; }
        public int Counferences { get; set; }
        public bool HasTask { get; set; }
        public bool HasNotes { get; set; }
        public List<KeyContacts> KeyContacts { get; set; }
        public List<IncidentAssets> IncidentAssets { get; set; }
        public bool IsKeyContact { get; set; }
        public string CompanyName { get; set; }
    }
}
