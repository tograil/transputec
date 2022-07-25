using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReassignTask
{
    public class ReassignTaskHandler : IRequestHandler<ReassignTaskRequest, ReassignTaskResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<ReassignTaskHandler> _logger;
        private readonly ReassignTaskValidator _reassignTaskValidator;
        public ReassignTaskHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<ReassignTaskHandler> logger, ReassignTaskValidator reassignTaskValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._reassignTaskValidator = reassignTaskValidator;
        }
        public async Task<ReassignTaskResponse> Handle(ReassignTaskRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ReassignTaskRequest));
            await _reassignTaskValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.ReassignTask(request);
            return result;
        }
    }
}
