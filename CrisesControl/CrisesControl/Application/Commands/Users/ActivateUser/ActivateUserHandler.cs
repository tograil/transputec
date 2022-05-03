using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ActivateUser
{
    public class ActivateUserHandler : IRequestHandler<ActivateUserRequest, ActivateUserResponse>
    {
        private readonly ActivateUserValidator _userValidator;
        private readonly IUserQuery _userQuery;

        public ActivateUserHandler(ActivateUserValidator userValidator, IUserQuery userQuery)
        {
            _userValidator = userValidator;
            _userQuery = userQuery;
        }

        public async Task<ActivateUserResponse> Handle(ActivateUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ActivateUserRequest));

            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _userQuery.ReactivateUser(request.QueriedUserId, cancellationToken);

            return result;
        }

        public Task<Unit> Handle(ActivateUserResponse request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
