using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.Repositories;
using FluentValidation;
using MediatR;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Reports.GetPingReportChart
{
    public class GetPingReportChartHandler : IRequestHandler<GetPingReportChartRequest, GetPingReportChartResponse>
    {
        private readonly IReportsRepository _reportRepository;
        private readonly GetPingReportChartValidator _getPingReportChartValidator;
        private readonly ILogger<GetPingReportChartHandler> _logger;
        private readonly ICurrentUser _currentUser;
        public GetPingReportChartHandler(IReportsRepository reportRepository, GetPingReportChartValidator getPingReportChartValidator, ILogger<GetPingReportChartHandler> logger, ICurrentUser currentUser)
        {
            this._reportRepository = reportRepository;
            this._getPingReportChartValidator = getPingReportChartValidator;
            this._logger = logger;
            this._currentUser = currentUser;
        }
        public async Task<GetPingReportChartResponse> Handle(GetPingReportChartRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Guard.Against.Null(request, nameof(GetPingReportChartRequest));
                await _getPingReportChartValidator.ValidateAndThrowAsync(request, cancellationToken);
                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;
                CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetStartEndDate(request.IsThisWeek, request.IsThisMonth, request.IsLastMonth, ref stDate, ref enDate, request.StartDate, request.EndDate);

                List<PingGroupChartCount> result = await _reportRepository.GetPingReportChart(stDate, enDate, 0, "PING", 0);
                int PingMinLimit = Convert.ToInt32(await _reportRepository.GetCompanyParameter("MIN_PING_KPI", _currentUser.CompanyId));

                int PingMaxLimit = Convert.ToInt32(await _reportRepository.GetCompanyParameter("MAX_PING_KPI", _currentUser.CompanyId));
                var mainresult = result.FirstOrDefault();
             
                if (result != null)
                {
                    
                    return new GetPingReportChartResponse
                    {
                        KPILimit = PingMinLimit,
                        KPIMaxLimit=PingMaxLimit,
                        ErrorCode = HttpStatusCode.OK,
                        Message ="Ping Chart Has been loaded"
                    };

                }
                return new GetPingReportChartResponse
                {
                    KPILimit = PingMinLimit,
                    KPIMaxLimit = PingMaxLimit,
                    ErrorCode = HttpStatusCode.NotFound,
                    Message = "No Data Found for Chart"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occure while seeding into the database {0}, {1}", ex.Message, ex.InnerException);
                return new GetPingReportChartResponse { };
            }
           
        }
    }
}
