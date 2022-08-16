using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentDetailsById
{
    public class GetActiveIncidentDetailsByIdHandler : IRequestHandler<GetActiveIncidentDetailsByIdRequest, GetActiveIncidentDetailsByIdResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetActiveIncidentDetailsByIdHandler> _logger;
        private readonly GetActiveIncidentDetailsByIdValidator _getActiveIncidentDetailsByIdValidator;
        public GetActiveIncidentDetailsByIdHandler(IIncidentQuery incidentQuery, ILogger<GetActiveIncidentDetailsByIdHandler> logger, GetActiveIncidentDetailsByIdValidator getActiveIncidentDetailsByIdValidator)
        {

            this._logger = logger;
            this._incidentQuery = incidentQuery;
            this._getActiveIncidentDetailsByIdValidator = getActiveIncidentDetailsByIdValidator;
        }
        public async Task<GetActiveIncidentDetailsByIdResponse> Handle(GetActiveIncidentDetailsByIdRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetActiveIncidentDetailsByIdRequest));
            await _getActiveIncidentDetailsByIdValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.GetActiveIncidentDetailsById(request);
            return result;
        }
    }
}
