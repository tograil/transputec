using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetLibIncident
{
    public class GetLibIncidentHandler : IRequestHandler<GetLibIncidentRequest, GetLibIncidentResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetLibIncidentHandler> _logger;
        private readonly GetLibIncidentValidator _getLibIncidentValidator;
        public GetLibIncidentHandler(IAdminQuery adminQuery, ILogger<GetLibIncidentHandler> logger, GetLibIncidentValidator getLibIncidentValidator)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
            this._getLibIncidentValidator = getLibIncidentValidator;
        }
        public async Task<GetLibIncidentResponse> Handle(GetLibIncidentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetLibIncidentRequest));

            await _getLibIncidentValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.GetLibIncident(request);
            return result;
        }
    }
}
