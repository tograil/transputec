using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUsers
{
    public class GetUsersHandler : IRequestHandler<GetUsersRequest,GetUsersResponse>
    {
        private readonly GetUsersValidator _userValidator;
        private readonly IUserQuery _userQuery;

        public GetUsersHandler(GetUsersValidator userValidator, 
            IUserRepository userService,
            IUserQuery userQuery)
        {
            _userValidator = userValidator;
            _userQuery = userQuery;
        }

        public async Task<GetUsersResponse> Handle(GetUsersRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUsersRequest));
            
            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var users = await _userQuery.GetUsers(request);
            return users;
        }
    }
}
