using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.UpdateCompanyParameters
{
    public class UpdateCompanyParametersRequest:IRequest<UpdateCompanyParametersResponse>
    {
        public int CompanyParametersId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
