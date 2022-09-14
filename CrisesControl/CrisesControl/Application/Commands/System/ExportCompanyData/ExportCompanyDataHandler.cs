using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.ExportCompanyData
{
    public class ExportCompanyDataHandler:IRequestHandler<ExportCompanyDataRequest, ExportCompanyDataResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<ExportCompanyDataHandler> _logger;

        public ExportCompanyDataHandler(ISystemQuery systemQuery, ILogger<ExportCompanyDataHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }

        public async Task<ExportCompanyDataResponse> Handle(ExportCompanyDataRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ExportCompanyDataRequest));
            var result = await _systemQuery.ExportCompanyData(request);
            return result;
        }
    }
}
