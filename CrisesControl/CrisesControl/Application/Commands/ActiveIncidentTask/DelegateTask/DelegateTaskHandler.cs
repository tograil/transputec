using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.DelegateTask
{
    public class DelegateTaskHandler:IRequestHandler<DelegateTaskRequest, DelegateTaskResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<DelegateTaskHandler> _logger;
        private readonly DelegateTaskValidator _delegateTaskValidator;
        public DelegateTaskHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<DelegateTaskHandler> logger, DelegateTaskValidator delegateTaskValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._delegateTaskValidator = delegateTaskValidator;
        }

        public async Task<DelegateTaskResponse> Handle(DelegateTaskRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DelegateTaskRequest));
            await _delegateTaskValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.DelegateTask(request);
            return result;
        }
    }
}
