using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.DeclineTask
{
    public class DeclineTaskHandler : IRequestHandler<DeclineTaskRequest, DeclineTaskResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<DeclineTaskHandler> _logger;
        private readonly DeclineTaskValidator _declineTaskValidator;
        public DeclineTaskHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<DeclineTaskHandler> logger, DeclineTaskValidator declineTaskValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._declineTaskValidator = declineTaskValidator;
        }
        public async Task<DeclineTaskResponse> Handle(DeclineTaskRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeclineTaskRequest));
            await _declineTaskValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.DeclineTask(request);
            return result;
        }
    }
}
