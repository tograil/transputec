using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.UpdateCompanyIncidentAction
{
    public class UpdateCompanyIncidentActionHandler : IRequestHandler<UpdateCompanyIncidentActionRequest, UpdateCompanyIncidentActionResponse>
    {
    
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<UpdateCompanyIncidentActionHandler> _logger;
        public UpdateCompanyIncidentActionHandler( IIncidentQuery incidentQuery, ILogger<UpdateCompanyIncidentActionHandler> logger)
        {
         
            this._logger = logger;
            this._incidentQuery = incidentQuery;
        }
        public async Task<UpdateCompanyIncidentActionResponse> Handle(UpdateCompanyIncidentActionRequest request, CancellationToken cancellationToken)
        {
                  
            var result = await _incidentQuery.UpdateCompanyIncidentAction(request);
            return result;
        }
    }
}
