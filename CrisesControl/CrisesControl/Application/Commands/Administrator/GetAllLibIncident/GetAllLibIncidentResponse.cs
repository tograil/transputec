using CrisesControl.Core.Administrator;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident
{
    public class GetAllLibIncidentResponse
    {
        public List<LibIncident> data { get; set; }
        public string Message { get; set; }
    }
}
