using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.ReactivateCompany
{
    public class ReactivateCompanyRequest:IRequest<ReactivateCompanyResponse>
    {
        public int ActivateReactivateCompanyId { get; set; }
    }
}
