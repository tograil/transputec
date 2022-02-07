using System;

namespace CrisesControl.Core.Models
{
    public partial class UserLocation1
    {
        public int UserLocationId { get; set; }
        public int UserId { get; set; }
        public int UserDeviceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset CreatedOnGmt { get; set; }
        public DateTimeOffset UserDeviceTime { get; set; }
        public string? LocationAddress { get; set; }
    }
}
