using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SegregationOtp
{
    public class SegregationOtpHandler : IRequestHandler<SegregationOtpRequest, SegregationOtpResponse>
    {
        private readonly ICompanyParametersQuery _companyParametersQuery;
        private readonly ILogger<SegregationOtpHandler> _logger;
        public SegregationOtpHandler(ILogger<SegregationOtpHandler> logger, ICompanyParametersQuery companyParametersQuery)
        {
            this._companyParametersQuery = companyParametersQuery;
            this._logger = logger;
        }
        public async Task<SegregationOtpResponse> Handle(SegregationOtpRequest request, CancellationToken cancellationToken)
        {
            var site = await _companyParametersQuery.SegregationOtp(request);
            return site;
        }
    }
}
