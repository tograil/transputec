using System;

namespace CrisesControl.Core.Models
{
    public partial class Job
    {
        public int JobId { get; set; }
        public string? JobType { get; set; }
        public string? JobName { get; set; }
        public string? JobDescription { get; set; }
        public string? CommandLine { get; set; }
        public string? CommandLineParams { get; set; }
        public string? ActionType { get; set; }
        public DateTimeOffset NextRunDate { get; set; }
        public string? NextRunTime { get; set; }
        public bool IsEnabled { get; set; }
        public bool Locked { get; set; }
        public string? LockedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int CompanyId { get; set; }
        public DateTimeOffset LastRunDateTime { get; set; }
        public int JobIncidentId { get; set; }
    }
}
