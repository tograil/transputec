using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class UserIncidentReportResponse
    {
        public int IncidentActivationId { get; set; }
        public string IncidentIcon { get; set; }
        public string IncidentName { get; set; }
        public int Severity { get; set; }
        public bool HasNotes { get; set; }
        public string IncidentLocation { get; set; }
        public string IncidentActivatedByFirstName
        {
            get
            {
                return IncidentActivatedBy?.Firstname ?? null;
            }
            set
            {
                if (IncidentActivatedBy == null)
                {
                    IncidentActivatedBy = new UserFullName();
                }
                if (IncidentActivatedBy.Firstname != value)
                {
                    IncidentActivatedBy.Firstname = value;
                }
            }
        }
        public string IncidentActivatedByLastName
        {
            get
            {
                return IncidentActivatedBy?.Lastname ?? null;
            }
            set
            {
                if (IncidentActivatedBy == null)
                {
                    IncidentActivatedBy = new UserFullName();
                }
                if (IncidentActivatedBy.Lastname != value)
                {
                    IncidentActivatedBy.Lastname = value;
                }
            }
        }
        [NotMapped]
        public UserFullName IncidentActivatedBy { get; set; }
        public DateTimeOffset IncidentActivatedOn { get; set; }
        public string IncidentLaunchByFirstName
        {
            get
            {
                return IncidentLaunchBy?.Firstname ?? null;
            }
            set
            {
                if (IncidentLaunchBy == null)
                {
                    IncidentLaunchBy = new UserFullName();
                }
                if (IncidentLaunchBy.Firstname != value)
                {
                    IncidentLaunchBy.Firstname = value;
                }
            }
        }
        public string IncidentLaunchByLastName
        {
            get
            {
                return IncidentLaunchBy?.Lastname ?? null;
            }
            set
            {
                if (IncidentLaunchBy == null)
                {
                    IncidentLaunchBy = new UserFullName();
                }
                if (IncidentLaunchBy.Lastname != value)
                {
                    IncidentLaunchBy.Lastname = value;
                }
            }
        }
        [NotMapped]
        public UserFullName IncidentLaunchBy { get; set; }
        public DateTimeOffset IncidentLaunchOn { get; set; }
        public int incidentCurrentStatus { get; set; }
        public int IncidentMessageCount { get; set; }
        public int AcknowledgedMessageCount { get; set; }
        public int UnAcknowledgedMessageCount { get; set; }
    }
}
