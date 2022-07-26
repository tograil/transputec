using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class IncidentReportResponse
    {
        public int IncidentActivationId { get; set; }
        public string IncidentIcon { get; set; }
        public int IncidentSeverity { get; set; }
        public string IncidentName { get; set; }
        public string IncidentLocation { get; set; }
        public string ActivatedByFirstName
        {
            get
            {
                return ActivatedBy?.Firstname ?? null;
            }
            set
            {
                if (ActivatedBy == null)
                {
                    ActivatedBy = new UserFullName();
                }
                if (ActivatedBy.Firstname != value)
                {
                    ActivatedBy.Firstname = value;
                }
            }
        }
        public string ActivatedByLastName
        {
            get
            {
                return ActivatedBy?.Lastname ?? null;
            }
            set
            {
                if (ActivatedBy == null)
                {
                    ActivatedBy = new UserFullName();
                }
                if (ActivatedBy.Lastname != value)
                {
                    ActivatedBy.Lastname = value;
                }
            }
        }
        [NotMapped]
        public UserFullName ActivatedBy { get; set; }
        public DateTimeOffset ActivatedOn { get; set; }
        public string LaunchedByFirstName
        {
            get
            {
                return LaunchedBy?.Firstname ?? null;
            }
            set
            {
                if (LaunchedBy == null)
                {
                    LaunchedBy = new UserFullName();
                }
                if (LaunchedBy.Firstname != value)
                {
                    LaunchedBy.Firstname = value;
                }
            }
        }
        public string LaunchedByLastName
        {
            get
            {
                return LaunchedBy?.Lastname ?? null;
            }
            set
            {
                if (LaunchedBy == null)
                {
                    LaunchedBy = new UserFullName();
                }
                if (LaunchedBy.Lastname != value)
                {
                    LaunchedBy.Lastname = value;
                }
            }
        }
        [NotMapped]
        public UserFullName LaunchedBy { get; set; }
        public DateTimeOffset LaunchedOn { get; set; }
        public string DeActivatedByFirstName
        {
            get
            {
                return DeactivatedBy?.Firstname ?? null;
            }
            set
            {
                if (DeactivatedBy == null)
                {
                    DeactivatedBy = new UserFullName();
                }
                if (DeactivatedBy.Firstname != value)
                {
                    DeactivatedBy.Firstname = value;
                }
            }
        }
        public string DeActivatedByLastName
        {
            get
            {
                return DeactivatedBy?.Lastname ?? null;
            }
            set
            {
                if (DeactivatedBy == null)
                {
                    DeactivatedBy = new UserFullName();
                }
                if (DeactivatedBy.Lastname != value)
                {
                    DeactivatedBy.Lastname = value;
                }
            }
        }
        [NotMapped]
        public UserFullName DeactivatedBy { get; set; }
        public DateTimeOffset DeactivatedOn { get; set; }
        public string ClosedByFirstName
        {
            get
            {
                return ClosedBy?.Firstname ?? null;
            }
            set
            {
                if (ClosedBy == null)
                {
                    ClosedBy = new UserFullName();
                }
                if (ClosedBy.Firstname != value)
                {
                    ClosedBy.Firstname = value;
                }
            }
        }
        public string ClosedByLastName
        {
            get
            {
                return ClosedBy?.Lastname ?? null;
            }
            set
            {
                if (ClosedBy == null)
                {
                    ClosedBy = new UserFullName();
                }
                if (ClosedBy.Lastname != value)
                {
                    ClosedBy.Lastname = value;
                }
            }
        }
        [NotMapped]
        public UserFullName ClosedBy { get; set; }
        public DateTimeOffset ClosedOn { get; set; }
        public int CurrentStatus { get; set; }
        public bool HasNotes { get; set; }
        public bool HasTask { get; set; }
        public List<IncidentReportKeyContactResponse> KeyContacts { get; set; }
    
}
}
