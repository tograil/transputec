using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAllActiveCompanyIncident
{
    public class GetAllActiveCompanyIncidentRequest:IRequest<GetAllActiveCompanyIncidentResponse>
    {
  
        public string Status { get; set; }
 
    }
}
