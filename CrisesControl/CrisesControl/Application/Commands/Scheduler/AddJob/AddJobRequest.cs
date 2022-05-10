using MediatR;

namespace CrisesControl.Api.Application.Commands.Scheduler.AddJob;

public class AddJobRequest : IRequest<AddJobResponse>
{
    public int JobId { get; set; }
    public string JobType { get; set; }
    
    public string JobName { get; set; }
    
    public string JobDescription { get; set; }
    
    public string CommandLine { get; set; }
    public string CommandLineParams { get; set; }
    
    public string ActionType { get; set; }
    
    public bool IsEnabled { get; set; }
    //---For JobSchedule-----------

    public int JobScheduleId { get; set; }
    public string FrequencyType { get; set; }
    public int FrequencyInterval { get; set; }
    public string FrequencySubDayType { get; set; }
    public int FrequencySubDayInterval { get; set; }
    public int RecurrenceFactor { get; set; }
    public DateTime ActiveStartDate { get; set; }
    public string ActiveStartTime { get; set; }
    public DateTime ActiveEndDate { get; set; }
    public string ActiveEndTime { get; set; }
    public bool IsActive { get; set; } //--To enable of disable a schedule
    public int JobIncidentId { get; set; }
}