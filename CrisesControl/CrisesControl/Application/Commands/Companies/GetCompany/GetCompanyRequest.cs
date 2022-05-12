using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetCompany {
    public class GetCompanyRequest : IRequest<GetCompanyResponse>{
        public int CompanyId { get; set; }
    }
    
}
