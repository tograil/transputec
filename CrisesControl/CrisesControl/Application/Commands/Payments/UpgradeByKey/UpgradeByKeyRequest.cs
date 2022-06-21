using MediatR;

namespace CrisesControl.Api.Application.Commands.Payments.UpgradeByKey
{
    public class UpgradeByKeyRequest:IRequest<UpgradeByKeyResponse>
    {
        public string IPAddress { get; set; }
        public string ActivationKey { get; set; }
        public int OutUserCompanyId { get; set; }
    }
}
