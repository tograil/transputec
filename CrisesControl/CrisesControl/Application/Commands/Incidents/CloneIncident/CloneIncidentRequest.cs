using CrisesControl.Core.Incidents;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.CloneIncident;

public class CloneIncidentRequest : IRequest<NewIncident>
{
    public int IncidentId { get; set; }
    public bool KeepKeyContact { get; set; }
    public bool KeepIncidentMessage { get; set; }
    public bool KeepTasks { get; set; }
    public bool KeepIncidentAsset { get; set; }
    public bool KeepTaskAssets { get; set; }
    public bool KeepTaskCheckList { get; set; }
    public bool KeepIncidentParticipants { get; set; }
    public int Status { get; set; }
}