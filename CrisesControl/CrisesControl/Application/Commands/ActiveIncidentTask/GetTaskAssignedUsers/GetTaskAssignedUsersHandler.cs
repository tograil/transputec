using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAssignedUsers
{
    public class GetTaskAssignedUsersHandler : IRequestHandler<GetTaskAssignedUsersRequest, GetTaskAssignedUsersResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<GetTaskAssignedUsersHandler> _logger;
        private readonly GetTaskAssignedUsersValidator _getTaskAssignedUsersValidator;
        public GetTaskAssignedUsersHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<GetTaskAssignedUsersHandler> logger, GetTaskAssignedUsersValidator getTaskAssignedUsersValidator)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            this._getTaskAssignedUsersValidator = getTaskAssignedUsersValidator;
        }
        public async Task<GetTaskAssignedUsersResponse> Handle(GetTaskAssignedUsersRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetTaskAssignedUsersRequest));
            await _getTaskAssignedUsersValidator.ValidateAndThrowAsync(request, cancellationToken); ;
            var result = await _activeIncidentQuery.GetTaskAssignedUsers(request);
            return result;
        }
    }
}
