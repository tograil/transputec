using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.IncidentStatusUpdate
{
    public class IncidentStatusUpdateResponse
    {
        public UpdateIncidentStatusReturn Data { get; set; }
        public string Message { get; set; }
    }
}
