using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.UpgradeRequest
{
    public class UpgradeRequest:IRequest<UpgradeResponse>
    {
        public int CompanyId { get; set; }
    }
}
