using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Billing;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile {
    public class GetPaymentProfileHandler : IRequestHandler<GetPaymentProfileRequest, GetPaymentProfileResponse> {

        private readonly IBillingQuery _billingQuery;

        public GetPaymentProfileHandler(IBillingQuery billingQuery) {
            _billingQuery = billingQuery;
        }

        public async Task<GetPaymentProfileResponse> Handle(GetPaymentProfileRequest request, CancellationToken cancellationToken) {
            Guard.Against.Null(request, nameof(GetPaymentProfileRequest));

            var pprofile = await _billingQuery.GetPaymentProfile(request);

            return pprofile;
        }
    }
}
