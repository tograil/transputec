using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCompanyIncidentType;

public class GetCompanyIncidentTypeResponse
{
    public List<IncidentTypeReturn> Data { get; set; }
    public string ErrorCode { get; set; }
}