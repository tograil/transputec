using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUser
{
    public class GetUserHandler : IRequestHandler<GetUserRequest, GetUserResponse>
    {

        private readonly GetUserValidator _userValidator;
        private readonly IUserQuery _userQuery;
        private readonly ILogger<GetUserHandler> _logger;

        public GetUserHandler(GetUserValidator userValidator, IUserQuery userService, ILogger<GetUserHandler> logger)
        {
            _userValidator = userValidator;
            _userQuery = userService;
            _logger = logger;
        }
        public async Task<GetUserResponse> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserRequest));
            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _userQuery.GetUser(request, cancellationToken);
            return response;
        }
    }
}
