using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SaveCascading
{
    public class SaveCascadingHandler : IRequestHandler<SaveCascadingRequest, SaveCascadingResponse>
    {
        private readonly ICompanyParametersQuery _companyParameterQuery;
        private readonly ILogger<SaveCascadingHandler> _logger;
        public SaveCascadingHandler(ICompanyParametersQuery companyParameterQuery, ILogger<SaveCascadingHandler> logger)
        {
            this._companyParameterQuery = companyParameterQuery;
            this._logger = logger;
        }
        public Task<SaveCascadingResponse> Handle(SaveCascadingRequest request, CancellationToken cancellationToken)
        {
            var result = _companyParameterQuery.SaveCascading(request);
            return result;
        }
    }
}
