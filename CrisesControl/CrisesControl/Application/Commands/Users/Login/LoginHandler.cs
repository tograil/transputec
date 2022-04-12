using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.Login
{
    public class LoginHandler: IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly LoginValidator _userValidator;
        private readonly IUserQuery _userQuery;

        public LoginHandler(LoginValidator userValidator, IUserQuery userQuery)
        {
            _userValidator = userValidator;
            _userQuery = userQuery;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(LoginRequest));

            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);

            var loginInfo = await _userQuery.GetLoggedInUserInfo(request, cancellationToken);

            return loginInfo;
        }
    }
}
