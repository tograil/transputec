using System;

namespace CrisesControl.Core.Models
{
    public partial class TrackMe
    {
        public int TrackMeId { get; set; }
        public int UserId { get; set; }
        public int UserDeviceId { get; set; }
        public int CompanyId { get; set; }
        public DateTimeOffset? TrackMeStarted { get; set; }
        public DateTimeOffset? TrackMeStopped { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public string? TrackType { get; set; }
        public int ActiveIncidentId { get; set; }
    }
}
