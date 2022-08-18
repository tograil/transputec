using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.DeleteCompanyIncidentAction
{
    public class DeleteCompanyIncidentActionHandler : IRequestHandler<DeleteCompanyIncidentActionRequest, DeleteCompanyIncidentActionResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<DeleteCompanyIncidentActionHandler> _logger;
        private readonly DeleteCompanyIncidentActionValidator _deleteCompanyIncidentActionValidator;
        public DeleteCompanyIncidentActionHandler(IIncidentQuery incidentQuery, ILogger<DeleteCompanyIncidentActionHandler> logger, DeleteCompanyIncidentActionValidator deleteCompanyIncidentValidator)
        {

            this._logger = logger;
            this._incidentQuery = incidentQuery;
            this._deleteCompanyIncidentActionValidator = deleteCompanyIncidentValidator;
        }
        public async Task<DeleteCompanyIncidentActionResponse> Handle(DeleteCompanyIncidentActionRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteCompanyIncidentActionRequest));
            await _deleteCompanyIncidentActionValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.DeleteCompanyIncidentAction(request);
            return result;
        }
    }
}
