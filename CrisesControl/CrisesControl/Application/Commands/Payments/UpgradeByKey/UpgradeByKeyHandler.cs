using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Payments.UpgradeByKey
{
    public class UpgradeByKeyHandler : IRequestHandler<UpgradeByKeyRequest, UpgradeByKeyResponse>
    {
        private readonly ILogger<UpgradeByKeyHandler> _logger;
        private readonly IPaymentQuery _paymentQuery;
        private readonly UpgradeKeyValidator _upgradeKeyValidator;
        public UpgradeByKeyHandler(ILogger<UpgradeByKeyHandler> logger, IPaymentQuery paymentQuery)
        {
            this._logger = logger;
            this._paymentQuery = paymentQuery;
        }
        public async Task<UpgradeByKeyResponse> Handle(UpgradeByKeyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpgradeByKeyRequest));
            await _upgradeKeyValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _paymentQuery.UpgradeByKey(request);
            return result;
        }
    }
}
