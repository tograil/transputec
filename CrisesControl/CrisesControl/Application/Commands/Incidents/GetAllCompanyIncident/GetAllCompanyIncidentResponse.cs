using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAllCompanyIncident;

public class GetAllCompanyIncidentResponse
{
    public List<IncidentList> Data { get; set; }
    public string ErrorCode { get; set; }
}