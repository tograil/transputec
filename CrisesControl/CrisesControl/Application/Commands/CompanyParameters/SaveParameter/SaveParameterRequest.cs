using CrisesControl.Core.CompanyParameters;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SaveParameter
{
    public class SaveParameterRequest:IRequest<SaveParameterResponse>
    {
         public int CompanyParametersId { get; set; }
        public List<Parameter> Parameters { get; set; }
    }
}
