using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.AddCompanyParameter
{
    public class AddCompanyParameterHandler : IRequestHandler<AddCompanyParameterRequest, AddCompanyParameterResponse>
    {
        private readonly ICompanyParametersQuery _companyParamentersQuery;
        private readonly ILogger<AddCompanyParameterHandler> _logger;
        public AddCompanyParameterHandler(ICompanyParametersQuery companyParamentersQuery, ILogger<AddCompanyParameterHandler> logger)
        {
            this._companyParamentersQuery = companyParamentersQuery;
            this._logger = logger;
        }
        public async Task<AddCompanyParameterResponse> Handle(AddCompanyParameterRequest request, CancellationToken cancellationToken)
        {
            var result = await _companyParamentersQuery.AddCompanyParameter(request);
            return result;
        }
    }
}
