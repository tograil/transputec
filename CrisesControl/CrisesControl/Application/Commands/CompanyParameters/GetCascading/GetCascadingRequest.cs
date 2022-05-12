using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading
{
    public class GetCascadingRequest: IRequest<GetCascadingResponse>
    {
        public int CompanyID { get; set; }
        public string PlanType { get; set; }
        public int PlanID { get; set; }
      
    }
}
