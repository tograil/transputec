using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.UpgradeRequest
{
    public class UpgradeRequestHandler : IRequestHandler<UpgradeRequest, UpgradeResponse>
    {
        private readonly ILogger<UpgradeRequestHandler> _logger;
        private readonly IRegisterQuery _registerQuery;
        private readonly UpgradeRequestValidator _upgradeRequestValidator;
        public UpgradeRequestHandler(ILogger<UpgradeRequestHandler> logger, IRegisterQuery registerQuery, UpgradeRequestValidator upgradeRequestValidator)
        {

        }

        public async Task<UpgradeResponse> Handle(UpgradeRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpgradeRequest));

            await _upgradeRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

            var customer = await _registerQuery.UpgradeRequest(request);

            return customer;
        }
    }
}
