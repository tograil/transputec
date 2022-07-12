using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.DumpReport
{
    public class DumpReportHandler : IRequestHandler<DumpReportRequest, DumpReportResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<DumpReportHandler> _logger;
        private readonly DumpReportValidator _dumpReportValidator;
        public DumpReportHandler(IAdminQuery adminQuery, ILogger<DumpReportHandler> logger, DumpReportValidator dumpReportValidator)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
            this._dumpReportValidator = dumpReportValidator;
        }
        public async Task<DumpReportResponse> Handle(DumpReportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DumpReportRequest));

            await _dumpReportValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.DumpReport(request);
            return result;
        }
    }
}
