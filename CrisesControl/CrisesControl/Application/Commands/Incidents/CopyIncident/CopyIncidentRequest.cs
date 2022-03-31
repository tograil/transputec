using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.CopyIncident;

public class CopyIncidentRequest : IRequest<bool>
{
    public string CompanyProfile { get; set; }
    public int Status { get; set; }
}