using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SendTaskUpdate
{
    public class SendTaskUpdateHandler : IRequestHandler<SendTaskUpdateRequest, SendTaskUpdateResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<SendTaskUpdateHandler> _logger;
        private readonly SendTaskUpdateValidator _sendTaskUpdateValidator;
        public SendTaskUpdateHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<SendTaskUpdateHandler> logger, SendTaskUpdateValidator sendTaskUpdateValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._sendTaskUpdateValidator = sendTaskUpdateValidator;
        }
        public async Task<SendTaskUpdateResponse> Handle(SendTaskUpdateRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SendTaskUpdateRequest));
            await _sendTaskUpdateValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.SendTaskUpdate(request);
            return result;
        }
    }
}
