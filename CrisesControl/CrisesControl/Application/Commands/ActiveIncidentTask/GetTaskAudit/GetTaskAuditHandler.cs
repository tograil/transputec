using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAudit
{
    public class GetTaskAuditHandler : IRequestHandler<GetTaskAuditRequest, GetTaskAuditResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<GetTaskAuditHandler> _logger;
        private readonly GetTaskAuditValidator _getTaskAuditValidator;
        public GetTaskAuditHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<GetTaskAuditHandler> logger, GetTaskAuditValidator getTaskAuditValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._getTaskAuditValidator = getTaskAuditValidator;
        }
        public async Task<GetTaskAuditResponse> Handle(GetTaskAuditRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetTaskAuditRequest));
            await _getTaskAuditValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.GetTaskAudit(request);
            return result;
        }
    }
}
