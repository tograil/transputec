using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetCompanyComms
{
    public class GetCompanyCommsHandler : IRequestHandler<GetCompanyCommsRequest, GetCompanyCommsResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<GetCompanyCommsHandler> _logger;

        public GetCompanyCommsHandler(ICompanyQuery companyQuery, ILogger<GetCompanyCommsHandler> logger)
        {
            this._companyQuery = companyQuery;
            this._logger = logger;
        }
        public async Task<GetCompanyCommsResponse> Handle(GetCompanyCommsRequest request, CancellationToken cancellationToken)
        {
            var result = await _companyQuery.GetCompanyComms(request);
            return result;
        }
    }
}
