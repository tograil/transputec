using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Payments.UpgradePackage
{
    public class UpgradePackageHandler : IRequestHandler<UpgradePackageRequest, UpgradePackageResponse>
    {
        private readonly ILogger<UpgradePackageHandler> _logger;
        private readonly IPaymentQuery _paymentQuery;

        public UpgradePackageHandler(ILogger<UpgradePackageHandler> logger, IPaymentQuery paymentQuery)
        {
            this._logger = logger;
            this._paymentQuery = paymentQuery;

        }
        public async Task<UpgradePackageResponse> Handle(UpgradePackageRequest request, CancellationToken cancellationToken)
        {
            var result = await _paymentQuery.UpgradePackage(request);
            return result;
        }
    }
}
