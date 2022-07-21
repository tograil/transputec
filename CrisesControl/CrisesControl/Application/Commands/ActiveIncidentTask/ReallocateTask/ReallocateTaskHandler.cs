using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReallocateTask
{
    public class ReallocateTaskHandler : IRequestHandler<ReallocateTaskRequest, ReallocateTaskResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<ReallocateTaskHandler> _logger;
        private readonly ReallocateTaskValidator _reallocateTaskValidator;
        public ReallocateTaskHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<ReallocateTaskHandler> logger, ReallocateTaskValidator reallocateTaskValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._reallocateTaskValidator = reallocateTaskValidator;
        }
        public async Task<ReallocateTaskResponse> Handle(ReallocateTaskRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ReallocateTaskRequest));
            await _reallocateTaskValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.ReallocateTask(request);
            return result;
        }
    }
}
