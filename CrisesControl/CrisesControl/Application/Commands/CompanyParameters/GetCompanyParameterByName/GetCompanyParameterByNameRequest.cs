using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyParameterByName
{
    public class GetCompanyParameterByNameRequest:IRequest<GetCompanyParameterByNameResponse>
    {
      public  string ParamName { get; set; }
            
      public  string CustomerId { get; set; }
    }
}
