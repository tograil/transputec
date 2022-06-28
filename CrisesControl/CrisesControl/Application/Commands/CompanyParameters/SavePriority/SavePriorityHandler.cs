using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SavePriority
{
    public class SavePriorityHandler : IRequestHandler<SavePriorityRequest, SavePriorityResponse>
    {
        private readonly ICompanyParametersQuery _companyParametersQuery;
        private readonly ILogger<SavePriorityHandler> _logger;
        public SavePriorityHandler(ILogger<SavePriorityHandler> logger, ICompanyParametersQuery companyParametersQuery)
        {
            this._companyParametersQuery = companyParametersQuery;
            this._logger = logger;
        }
        public async Task<SavePriorityResponse> Handle(SavePriorityRequest request, CancellationToken cancellationToken)
        {
            var site = await _companyParametersQuery.SavePriority(request);
            return site;
        }
    }
}
