using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.CompanyStatsAdmin
{
    public class CompanyStatsAdminHandler:IRequestHandler<CompanyStatsAdminRequest, CompanyStatsAdminResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<CompanyStatsAdminHandler> _logger;

        public CompanyStatsAdminHandler(ISystemQuery systemQuery, ILogger<CompanyStatsAdminHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }

        public async Task<CompanyStatsAdminResponse> Handle(CompanyStatsAdminRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CompanyStatsAdminRequest));
            var result = await _systemQuery.CompanyStatsAdmin(request);
            return result;
        }
    }
}
