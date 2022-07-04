using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.DeactivateCompany
{
    public class DeactivateCompanyRequest:IRequest<DeactivateCompanyResponse>
    {
        public int TargetCompanyID { get; set; }
    
       
    }
}
