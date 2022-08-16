using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.DeleteCompanyIncident
{
    public class DeleteCompanyIncidentHandler : IRequestHandler<DeleteCompanyIncidentRequest, DeleteCompanyIncidentResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<DeleteCompanyIncidentHandler> _logger;
        private readonly DeleteCompanyIncidentValidator _deleteCompanyIncidentValidator;
        public DeleteCompanyIncidentHandler(IIncidentQuery incidentQuery, ILogger<DeleteCompanyIncidentHandler> logger, DeleteCompanyIncidentValidator deleteCompanyIncidentValidator)
        {

            this._logger = logger;
            this._incidentQuery = incidentQuery;
            this._deleteCompanyIncidentValidator = deleteCompanyIncidentValidator;
        }
        public async Task<DeleteCompanyIncidentResponse> Handle(DeleteCompanyIncidentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteCompanyIncidentRequest));
            await _deleteCompanyIncidentValidator.ValidateAndThrowAsync(request,cancellationToken);
            var result = await _incidentQuery.DeleteCompanyIncident(request);
            return result;
        }
    }
}
