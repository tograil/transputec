using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SaveCompanyFTP
{
    public class SaveCompanyFTPHandler : IRequestHandler<SaveCompanyFTPRequest, SaveCompanyFTPResponse>
    {
        private readonly ICompanyParametersQuery _companyParametersQuery;
        private readonly ILogger<SaveCompanyFTPHandler> _logger;
        public SaveCompanyFTPHandler(ILogger<SaveCompanyFTPHandler> logger, ICompanyParametersQuery companyParametersQuery)
        {
            this._companyParametersQuery = companyParametersQuery;
            this._logger = logger;
        }
        public async Task<SaveCompanyFTPResponse> Handle(SaveCompanyFTPRequest request, CancellationToken cancellationToken)
        {
            var site = await _companyParametersQuery.SaveCompanyFTP(request);

            return site;
        }
    }
}
