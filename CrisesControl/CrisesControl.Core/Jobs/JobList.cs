using CrisesControl.Core.Groups;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;

namespace CrisesControl.Core.Jobs
{
    public class JobList
    {
        public int JobID { get; set; }
        public string JobType { get; set; }
        public string JobName { get; set; }
        public string JobDescription { get; set; }
        public string CommandLine { get; set; }
        public string CommandLineParams { get; set; }
        public string ActionType { get; set; }
        public DateTimeOffset NextRunDate { get; set; }
        public string NextRunTime { get; set; }
        public bool IsEnabled { get; set; }
        public bool Locked { get; set; }
        public string LockedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int ScheduleID { get; set; }
        public string FrequencyType { get; set; }
        public int FrequencyInterval { get; set; }
        public string FrequencySubDayType { get; set; }
        public int FrequencySubDayInterval { get; set; }
        public int RecurrenceFactor { get; set; }
        public DateTimeOffset ActiveStartDate { get; set; }
        public string ActiveStartTime { get; set; }
        public DateTimeOffset SchedulerNextRunDate { get; set; }
        public string SchedulerNextRunTime { get; set; }

        public DateTimeOffset JActiveEndDate { get; set; }
        public string ActiveEndTime { get; set; }
        public bool IsActive { get; set; }
        public List<User> Users { get; set; }
        public List<Group> Groups { get; set; }

    }
}
