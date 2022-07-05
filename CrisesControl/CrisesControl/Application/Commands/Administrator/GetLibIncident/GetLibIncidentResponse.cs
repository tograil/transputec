using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.GetLibIncident
{
    public class GetLibIncidentResponse
    {
        public AdminLibIncident Data { get; set; }
        public string Message { get; set; }
    }
}
