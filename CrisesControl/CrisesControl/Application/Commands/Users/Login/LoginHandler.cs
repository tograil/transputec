using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.Login
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IUserQuery _userQuery;
        private readonly ILogger<LoginHandler> _logger;
        private readonly LoginValidator _validationUser;
        public LoginHandler(IUserQuery userQuery, ILogger<LoginHandler> logger, LoginValidator validationUser)
        {
            this._logger = logger;
            this._userQuery = userQuery;
            this._validationUser = validationUser;
        }
        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(LoginRequest));
            await _validationUser.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _userQuery.GetLoggedInUserInfo(request, cancellationToken);
            return response;
        }
    }
}
