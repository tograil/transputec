namespace CrisesControl.Api.Application.Commands.Scheduler.AddJob;

public class AddJobResponse
{
    public bool Success { get; set; }
    public int JobId { get; set; }
    public int JobScheduleId { get; set; }
}