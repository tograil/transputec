using MediatR;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetAllExTrigger
{
    public class GetAllExTriggerRequest: IRequest<GetAllExTriggerResponse>
    {
         public int CompanyID { get; set; } 
         public int UserID { get; set; }    
    }
}
