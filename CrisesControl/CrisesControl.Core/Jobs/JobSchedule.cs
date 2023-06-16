using System;

namespace CrisesControl.Core.Jobs
{
    public partial class JobSchedule
    {
        public int ScheduleId { get; set; }
        public int JobId { get; set; }
        public string? FrequencyType { get; set; }
        public int FrequencyInterval { get; set; }
        public string? FrequencySubDayType { get; set; }
        public int FrequencySubDayInterval { get; set; }
        public int RecurrenceFactor { get; set; }
        public DateTimeOffset ActiveStartDate { get; set; }
        public string? ActiveStartTime { get; set; }
        public DateTimeOffset ActiveEndDate { get; set; }
        public string? ActiveEndTime { get; set; }
        public DateTimeOffset NextRunDate { get; set; }
        public string? NextRunTime { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}
