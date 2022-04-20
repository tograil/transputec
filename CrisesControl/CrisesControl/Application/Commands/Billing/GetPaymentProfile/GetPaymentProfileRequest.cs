using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile {
    public class GetPaymentProfileRequest :  IRequest<GetPaymentProfileResponse>{
        public int CompanyId { get; set; }
    }
}
