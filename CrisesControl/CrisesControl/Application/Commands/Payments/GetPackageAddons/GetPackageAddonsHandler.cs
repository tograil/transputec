using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Payments.GetPackageAddons
{
    public class GetPackageAddonsHandler : IRequestHandler<GetPackageAddonsRequest, GetPackageAddonsResponse>
    {
        private readonly ILogger<GetPackageAddonsHandler> _logger;
        private readonly IPaymentQuery _paymentQuery;

        public GetPackageAddonsHandler(ILogger<GetPackageAddonsHandler> logger, IPaymentQuery paymentQuery)
        {
            this._logger = logger;
            this._paymentQuery = paymentQuery;

        }
        public async Task<GetPackageAddonsResponse> Handle(GetPackageAddonsRequest request, CancellationToken cancellationToken)
        {
            var result = await _paymentQuery.GetPackageAddons(request);
            return result;
        }
    }
}
