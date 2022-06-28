using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SaveParameter
{
    public class SaveParameterHandler : IRequestHandler<SaveParameterRequest, SaveParameterResponse>
    {
        private readonly ICompanyParametersQuery _companyParamentersQuery;
        private readonly ILogger<SaveParameterHandler> _logger;
        public SaveParameterHandler(ICompanyParametersQuery companyParamentersQuery, ILogger<SaveParameterHandler> logger)
        {
            this._companyParamentersQuery = companyParamentersQuery;
            this._logger = logger;
        }
        public async Task<SaveParameterResponse> Handle(SaveParameterRequest request, CancellationToken cancellationToken)
        {
            var result = await _companyParamentersQuery.SaveParameter(request);
            return result;
        }
    }
}
