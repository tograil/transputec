using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.DeleteCompany
{
    public class DeleteCompanyRequest:IRequest<DeleteCompanyResponse>
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string GUID { get; set; }
        public string DeleteType { get; set; }

    }
}
