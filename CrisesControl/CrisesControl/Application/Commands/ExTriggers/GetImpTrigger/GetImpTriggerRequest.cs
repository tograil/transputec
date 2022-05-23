using MediatR;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetImpTrigger
{
    public class GetImpTriggerRequest:IRequest<GetImpTriggerResponse>
    {
        public int CompanyID { get; set; }
        public int UserID { get; set; }
    }
}
