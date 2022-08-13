using CrisesControl.Core.Payments;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Payments.UpdateCompanyPaymentProfile
{
    public class UpdateCompanyPaymentProfileRequest:IRequest<UpdateCompanyPaymentProfileResponse>
    {
        public UpdateCompanyPaymentProfileModel Model { get; set; }
    }
}
