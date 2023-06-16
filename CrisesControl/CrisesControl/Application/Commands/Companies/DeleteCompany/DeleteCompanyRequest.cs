using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.DeleteCompany
{
    public class DeleteCompanyRequest:IRequest<DeleteCompanyResponse>
    {
        public int TargetCompanyID { get; set; }

    }
}
