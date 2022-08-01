using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask
{
    public class AcceptTaskHandler:IRequestHandler<AcceptTaskRequest, AcceptTaskResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<AcceptTaskHandler> _logger;
        private readonly AcceptTaskValidator _acceptTaskValidator;
        public AcceptTaskHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<AcceptTaskHandler> logger, AcceptTaskValidator acceptTaskValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._acceptTaskValidator = acceptTaskValidator;
        }
        public async Task<AcceptTaskResponse> Handle(AcceptTaskRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(AcceptTaskRequest));
            await _acceptTaskValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.AcceptTask(request);
            return result;
        }
    }
}

