using AutoMapper;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Reports.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetTrackingUserCount
{
    public class GetTrackingUserCountHandler : IRequestHandler<GetTrackingUserCountRequest, GetTrackingUserCountResponse>
    {
        
        private readonly ILogger<GetTrackingUserCountHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        public GetTrackingUserCountHandler(IMapper mapper, ILogger<GetTrackingUserCountHandler> logger, IReportsQuery reportsQuery)
        {
          
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetTrackingUserCountResponse> Handle(GetTrackingUserCountRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportsQuery.GetTrackingUserCount(request);
            return result;
        }
    }
}
