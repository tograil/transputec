using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.DeleteCascading
{
    public class DeleteCascadingRequest:IRequest<DeleteCascadingResponse>
    {
        public int PlanID { get; set; }
       
        public int CompanyId { get; set; }

    }
}
