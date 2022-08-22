using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetCompanyObject
{
    public class GetCompanyObjectHandler : IRequestHandler<GetCompanyObjectRequest, GetCompanyObjectResponse>
    {

        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<GetCompanyObjectHandler> _logger;

        public GetCompanyObjectHandler(ICompanyQuery companyQuery, ILogger<GetCompanyObjectHandler> logger)
        {
            this._companyQuery = companyQuery;
            this._logger = logger;
        }
        public async Task<GetCompanyObjectResponse> Handle(GetCompanyObjectRequest request, CancellationToken cancellationToken)
        {
            var result = await _companyQuery.GetCompanyObject(request);
            return result;
        }
    }
}
