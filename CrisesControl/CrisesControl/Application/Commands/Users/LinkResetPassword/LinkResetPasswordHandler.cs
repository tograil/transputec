using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.LinkResetPassword
{
    public class LinkResetPasswordHandler : IRequestHandler<LinkResetPasswordRequest, LinkResetPasswordResponse>
    {
        private readonly IUserQuery _userQuery;
        private readonly ILogger<LinkResetPasswordHandler> _logger;
        private readonly LinkResetPasswordValidator _validationResetPassword;
        public LinkResetPasswordHandler(IUserQuery userQuery,ILogger<LinkResetPasswordHandler> logger, LinkResetPasswordValidator validationReset)
        {
            this._logger = logger;
            this._userQuery = userQuery;
            this._validationResetPassword = validationReset;
        }
        public async Task<LinkResetPasswordResponse> Handle(LinkResetPasswordRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(LinkResetPasswordRequest));
            await _validationResetPassword.ValidateAndThrowAsync(request, cancellationToken);
             var result = await _userQuery.LinkResetPassword(request);
            return result;
           
        }
    }
}
