using CrisesControl.Core.Payments;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Payments.UpgradePackage
{
    public class UpgradePackageRequest:IRequest<UpgradePackageResponse>
    {
        public UpdateCompanyPaymentProfileModel Model { get; set; }
    }
}
