using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.AppInvitation
{
    public class AppInvitationHandler : IRequestHandler<AppInvitationRequest, AppInvitationResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly ILogger<AppInvitationHandler> _logger;
        private readonly AppInvitationValidator _appInvitationValidator;
        public AppInvitationHandler(IReportsQuery reportsQuery, AppInvitationValidator appInvitationValidator, ILogger<AppInvitationHandler> logger)
        {
            this._appInvitationValidator = appInvitationValidator;
            this._reportsQuery = reportsQuery;
            this._logger = logger;
        }
        public async Task<AppInvitationResponse> Handle(AppInvitationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(AppInvitationRequest));
            await _appInvitationValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.AppInvitation(request);
            return result;
        }
    }
}
