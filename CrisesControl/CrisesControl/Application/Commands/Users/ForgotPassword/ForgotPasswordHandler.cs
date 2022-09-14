using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ForgotPassword
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordRequest, ForgotPasswordResponse>
    {
        private readonly ForgotPasswordValidator _validationPassword;
        private readonly IUserQuery _userQuery;
        private readonly ILogger<ForgotPasswordHandler> _logger;
        public ForgotPasswordHandler(IUserQuery userQuery, ForgotPasswordValidator validationPassword, ILogger<ForgotPasswordHandler> logger)
        {
            this._userQuery = userQuery;
            this._validationPassword = validationPassword;
            this._logger = logger;
        }
        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ForgotPasswordRequest));
            await _validationPassword.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _userQuery.ForgotPassword(request);
            return result;
        }
    }
}
