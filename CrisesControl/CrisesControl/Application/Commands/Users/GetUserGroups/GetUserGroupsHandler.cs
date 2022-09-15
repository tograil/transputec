using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserGroups
{
    public class GetUserGroupsHandler : IRequestHandler<GetUserGroupsRequest, GetUserGroupsResponse>
    {
        private readonly GetUserGroupsValidator _userValidator;
        private readonly IUserQuery _userQuery;
        private readonly ILogger<GetUserGroupsHandler> _logger;

        public GetUserGroupsHandler(GetUserGroupsValidator userValidator, IUserQuery userService, ILogger<GetUserGroupsHandler> logger)
        {
            _userValidator = userValidator;
            _userQuery = userService;
            _logger = logger;
        }
        public async Task<GetUserGroupsResponse> Handle(GetUserGroupsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserGroupsRequest));
            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _userQuery.GetUserGroups(request);
            return response;
        }
    }
}
