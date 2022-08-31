using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Payments.GetCompanyPackageItems
{
    public class GetCompanyPackageItemsHandler : IRequestHandler<GetCompanyPackageItemsRequest, GetCompanyPackageItemsResponse>
    {
        private readonly ILogger<GetCompanyPackageItemsHandler> _logger;
        private readonly IPaymentQuery _paymentQuery;

        public GetCompanyPackageItemsHandler(ILogger<GetCompanyPackageItemsHandler> logger, IPaymentQuery paymentQuery)
        {
            this._logger = logger;
            this._paymentQuery = paymentQuery;

        }
        public async Task<GetCompanyPackageItemsResponse> Handle(GetCompanyPackageItemsRequest request, CancellationToken cancellationToken)
        {
            var result = await _paymentQuery.GetCompanyPackageItems(request);
            return result;
        }
    }
}
