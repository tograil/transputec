using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCompanySOS
{
    public class GetCompanySOSResponse
    {
        public IncidentDetails Data { get; set; }
        public string Message { get; set; }
    }
}
