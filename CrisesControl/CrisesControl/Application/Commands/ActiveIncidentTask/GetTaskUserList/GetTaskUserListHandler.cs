using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskUserList
{
    public class GetTaskUserListHandler : IRequestHandler<GetTaskUserListRequest, GetTaskUserListResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<GetTaskUserListHandler> _logger;
        private readonly GetTaskUserListValidator _getTaskUserListValidator;
        public GetTaskUserListHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<GetTaskUserListHandler> logger, GetTaskUserListValidator getTaskUserListValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._getTaskUserListValidator = getTaskUserListValidator;
        }
        public async Task<GetTaskUserListResponse> Handle(GetTaskUserListRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetTaskUserListRequest));
            await _getTaskUserListValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.GetTaskUserList(request);
            return result;
        }
    }
}
