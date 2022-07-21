using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveCheckListResponse
{
    public class SaveActiveCheckListResponseHandler : IRequestHandler<SaveActiveCheckListResponseRequest, SaveActiveCheckListResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<SaveActiveCheckListResponseHandler> _logger;
        private readonly SaveActiveCheckListResponseValidator _saveActiveCheckListValidator;
        public SaveActiveCheckListResponseHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<SaveActiveCheckListResponseHandler> logger, SaveActiveCheckListResponseValidator saveActiveCheckListValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._saveActiveCheckListValidator = saveActiveCheckListValidator;
        }
        public async Task<SaveActiveCheckListResponse> Handle(SaveActiveCheckListResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveActiveCheckListResponseRequest));
            await _saveActiveCheckListValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.SaveActiveCheckListResponse(request);
            return result;
        }
    }
    }

