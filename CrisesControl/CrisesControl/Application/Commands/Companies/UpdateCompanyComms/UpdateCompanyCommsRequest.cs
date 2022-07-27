using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyComms
{
    public class UpdateCompanyCommsRequest:IRequest<UpdateCompanyCommsResponse>
    {
        public int[] MethodId { get; set; }
        public int[] BillingUsers { get; set; }
        public int CurrentSuperUser { get; set; }
        public string Source { get; set; }
    }
}
