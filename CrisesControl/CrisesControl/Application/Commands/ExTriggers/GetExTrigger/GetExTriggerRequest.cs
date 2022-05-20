using MediatR;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetExTrigger
{
    public class GetExTriggerRequest: IRequest<GetExTriggerResponse>
    {
        public int ExTriggerID { get; set; }
        public  int CompanyID { get; set; }
    }
}
