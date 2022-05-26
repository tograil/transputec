using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.CreateIncidentTaskHeader;

public class CreateIncidentTaskHeaderRequest : IRequest<CreateIncidentTaskHeaderResponse>
{
    public int IncidentId { get; set; }
    public int TaskHeaderId { get; set; }
    public decimal Rto { get; set; }
    public decimal Rpo { get; set; }
    public DateTimeOffset NextReviewDate { get; set; }
    //TODO: Change ReviewFrequency into enum
    public string ReviewFrequency { get; set; }
    public bool SendReminder { get; set; }
    public int Author { get; set; }
    public bool IsActive { get; set; }
}