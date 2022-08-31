using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Payments.UpdateCompanyPaymentProfile
{
    public class UpdateCompanyPaymentProfileHandler : IRequestHandler<UpdateCompanyPaymentProfileRequest, UpdateCompanyPaymentProfileResponse>
    {
        private readonly ILogger<UpdateCompanyPaymentProfileHandler> _logger;
        private readonly IPaymentQuery _paymentQuery;

        public UpdateCompanyPaymentProfileHandler(ILogger<UpdateCompanyPaymentProfileHandler> logger, IPaymentQuery paymentQuery)
        {
            this._logger = logger;
            this._paymentQuery = paymentQuery;

        }
        public async Task<UpdateCompanyPaymentProfileResponse> Handle(UpdateCompanyPaymentProfileRequest request, CancellationToken cancellationToken)
        {
            var result = await _paymentQuery.UpdateCompanyPaymentProfile(request);
            return result;
        }
    }
}
