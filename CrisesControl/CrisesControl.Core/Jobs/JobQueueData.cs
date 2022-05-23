using System;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.Models;
using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.Core.Jobs;

public record JobQueueData
{
    public Job JobData { get; set; }

    public int JobScheduleId { get; set; }

    public JobType JobType { get; set; }

    public CcBase JobModel { get; set; }

    public JobFrequencyType FrequencyType { get; set; }

    public int FrequencyInterval { get; set; }

    public JobSubDayType JobSubDayType { get; set; }

    public int FrequencySubInterval { get; set; }

    public string TimeZone { get; set; }

    public DateTimeOffset? StartDate { get; set; }

}