using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Administrator.GetLibIncidentType
{
    public class GetLibIncidentTypeResponse
    {
        public List<LibIncidentType> Data { get; set; }
        public string Message { get; set; }
    }
}
