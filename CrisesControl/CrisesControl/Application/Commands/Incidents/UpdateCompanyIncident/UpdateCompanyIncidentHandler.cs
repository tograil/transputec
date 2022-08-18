using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.UpdateCompanyIncident
{
    public class UpdateCompanyIncidentHandler : IRequestHandler<UpdateCompanyIncidentRequest, UpdateCompanyIncidentResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<UpdateCompanyIncidentHandler> _logger;
        private readonly UpdateCompanyIncidentValidator _updateCompanyIncidentValidator;
        public UpdateCompanyIncidentHandler(IIncidentQuery incidentQuery, ILogger<UpdateCompanyIncidentHandler> logger, UpdateCompanyIncidentValidator updateCompanyIncidentValidator)
        {
            this._incidentQuery = incidentQuery;
            this._logger = logger;
            this._updateCompanyIncidentValidator = updateCompanyIncidentValidator;
        }
        public async Task<UpdateCompanyIncidentResponse> Handle(UpdateCompanyIncidentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateCompanyIncidentRequest));

            await _updateCompanyIncidentValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.UpdateCompanyIncident(request);
            return result;
        }
    }
}
