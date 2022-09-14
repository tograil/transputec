using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ResetPassword
{
    public class ResetPasswordHandler:IRequestHandler<ResetPasswordRequest,ResetPasswordResponse>
    {
        private readonly IUserQuery _userQuery;
        private readonly ILogger<ResetPasswordHandler> _logger;
        private readonly ResetPasswordValidator _validationReset;
        public ResetPasswordHandler(IUserQuery userQuery, ILogger<ResetPasswordHandler> logger, ResetPasswordValidator validationReset)
        {
            this._logger = logger;
            this._userQuery = userQuery;
            this._validationReset = validationReset;
        }

        public async Task<ResetPasswordResponse> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request,nameof(ResetPasswordRequest));
            await _validationReset.ValidateAndThrowAsync(request, cancellationToken);
            var result=await _userQuery.ResetPassword(request);
            return result;
        }
    }
}
