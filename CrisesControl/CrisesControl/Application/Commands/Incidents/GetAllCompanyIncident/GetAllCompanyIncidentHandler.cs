using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAllCompanyIncident
{
    public class GetAllCompanyIncidentHandler:IRequestHandler<GetAllCompanyIncidentRequest, GetAllCompanyIncidentResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        public GetAllCompanyIncidentHandler(IIncidentQuery incidentQuery)
        {
            _incidentQuery = incidentQuery;
        }

        public async Task<GetAllCompanyIncidentResponse> Handle(GetAllCompanyIncidentRequest request, CancellationToken cancellationToken)
        {
         var result=await  _incidentQuery.GetAllCompanyIncident(request);
            return null;
        }
    }
}
