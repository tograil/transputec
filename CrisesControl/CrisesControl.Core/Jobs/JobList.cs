using CrisesControl.Core.GroupAggregate;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Jobs
{
    public class JobList
    {
        public int JobID { get; set; }
        public string Jobs_JobType { get; set; }
        public string Jobs_JobName { get; set; }
        public string Jobs_JobDescription { get; set; }
        public string Jobs_CommandLine { get; set; }
        public string Jobs_CommandLineParams { get; set; }
        public string Jobs_ActionType { get; set; }
        public DateTimeOffset Jobs_NextRunDate { get; set; }
        public string Jobs_NextRunTime { get; set; }
        public bool Jobs_IsEnabled { get; set; }
        public bool Jobs_Locked { get; set; }
        public string Jobs_LockedBy { get; set; }
        public DateTimeOffset Jobs_CreatedDate { get; set; }
        public DateTimeOffset Jobs_UpdatedDate { get; set; }
        public int Jobs_UpdatedBy { get; set; }
        public int JobSchedule_ScheduleID { get; set; }
        public string JobSchedule_FrequencyType { get; set; }
        public int JobSchedule_FrequencyInterval { get; set; }
        public string JobSchedule_FrequencySubDayType { get; set; }
        public int JobSchedule_FrequencySubDayInterval { get; set; }
        public int JobSchedule_RecurrenceFactor { get; set; }
        public DateTimeOffset JobSchedule_ActiveStartDate { get; set; }
        public string JobSchedule_ActiveStartTime { get; set; }

        public DateTimeOffset JobSchedule_ActiveEndDate { get; set; }
        public string JobSchedule_ActiveEndTime { get; set; }

        public DateTimeOffset JobSchedule_NextRunDate { get; set; }

        public string JobSchedule_NextRunTime { get; set; }
        public bool JobSchedule_IsActive { get; set; }
        public List<User> Users { get; set; }
        public List<Group> Groups { get; set; }

    }
}
