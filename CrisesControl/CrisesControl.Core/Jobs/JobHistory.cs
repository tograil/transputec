using System;

namespace CrisesControl.Core.Jobs
{
    public partial class JobHistory
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string? Message { get; set; }
        public int Status { get; set; }
        public DateTimeOffset RunDate { get; set; }
        public int RunDuration { get; set; }
        public int IncidentActivationId { get; set; }
        public int PingMessageId { get; set; }
    }
}
