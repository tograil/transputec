using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident
{
    public class GetAllLibIncidentResponse
    {
        public List<LibIncident> data { get; set; }
        public string Message { get; set; }
    }
}
