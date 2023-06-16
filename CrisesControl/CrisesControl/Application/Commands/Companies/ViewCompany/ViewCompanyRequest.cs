using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.ViewCompany
{
    public class ViewCompanyRequest:IRequest<ViewCompanyResponse>
    {
        public int CompanyId { get; set; }
    }
}
