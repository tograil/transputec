using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile {
    public class GetPaymentProfileHandler : IRequestHandler<GetPaymentProfileRequest, GetPaymentProfileResponse> {

        private readonly IBillingRepository _billingQuery;

        public GetPaymentProfileHandler(IBillingRepository billingQuery) {
            _billingQuery = billingQuery;
        }

        public async Task<GetPaymentProfileResponse> Handle(GetPaymentProfileRequest request, CancellationToken cancellationToken) 
        {
            Guard.Against.Null(request, nameof(GetPaymentProfileRequest));

            var pprofile = await _billingQuery.GetPaymentProfile(request.CompanyId);
            var response = new GetPaymentProfileResponse();
            response.PaidServices = pprofile.PaidServices;
            response.Profile = pprofile.Profile;

            return response;
        }
    }
}
