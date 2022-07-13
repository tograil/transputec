using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncidentType
{
    public class GetAllLibIncidentTypeResponse
    {
        public List<LibIncidentType> Data { get; set; }
        public string Message { get; set; }
    }
}
