using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports {
    public class SOSItem {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string ISDCode { get; set; }
        public string PrimaryEmail { get; set; }
        public string UserLocationLat { get; set; }
        public string UserLocationLong { get; set; }
        public string ResponseLabel { get; set; }
        public DateTimeOffset ResponseTime { get; set; }
        [NotMapped]
        public bool Completed { get; set; }
        public string UserPhoto { get; set; }
        [NotMapped]
        public string IncidentName { get; set; }
        [NotMapped]
        public string IncidentIcon { get; set; }
        [NotMapped]
        public int Severity { get; set; }
        [NotMapped]
        public string Location_Name { get; set; }
        public int SOSAlertID { get; set; }
        public int ActiveIncidentID { get; set; }
        [NotMapped]
        public bool IsSOS { get; set; }
        public int CallbackOption { get; set; }
        public DateTimeOffset LastLocationUpdate { get; set; }
    }
}
