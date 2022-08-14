using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentDetailsById
{
    public class GetActiveIncidentDetailsByIdResponse
    {
        public UpdateIncidentStatus Data { get; set; }
        public string Message { get; set; }
    }
}
