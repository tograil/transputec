using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters {
    public class GetAllCompanyParametersRequest : IRequest<GetAllCompanyParametersResponse>{
        public int CompanyId { get; set; }
    }
}
