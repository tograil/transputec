using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.AddCompanyParameter
{
    public class AddCompanyParameterRequest:IRequest<AddCompanyParameterResponse>
    {
        public string Name { get; set; }
        public string Value { get; set; }
    
    }
}
