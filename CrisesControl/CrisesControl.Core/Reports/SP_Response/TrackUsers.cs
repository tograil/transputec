using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports.SP_Response
{
    public class TrackUsers
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
        public string ISDCode { get; set; }
        public string MobileNo { get; set; }
        public string PrimaryEmail { get; set; }
        public DateTimeOffset TrackMeStarted { get; set; }
        public DateTimeOffset TrackMeStopped { get; set; }
        public string TrackType { get; set; }
        public string UserPhoto { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceType { get; set; }
        public int UserDeviceID { get; set; }
        public int TrackMeID { get; set; }
    }
}
