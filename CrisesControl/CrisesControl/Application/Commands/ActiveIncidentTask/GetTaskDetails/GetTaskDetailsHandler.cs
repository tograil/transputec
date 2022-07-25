using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskDetails
{
    public class GetTaskDetailsHandler : IRequestHandler<GetTaskDetailsRequest, GetTaskDetailsResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<GetTaskDetailsHandler> _logger;
        private readonly GetTaskDetailsValidator _getTaskDetailsValidator;
        public GetTaskDetailsHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<GetTaskDetailsHandler> logger, GetTaskDetailsValidator getTaskAuditValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._getTaskDetailsValidator = getTaskAuditValidator;
        }
        public async Task<GetTaskDetailsResponse> Handle(GetTaskDetailsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetTaskDetailsRequest));
            await _getTaskDetailsValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.GetTaskDetails(request);
            return result;
        }
    }
}
