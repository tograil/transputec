using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats {
    public class GetIncidentPingStatsRequest : IRequest<GetIncidentPingStatsResponse>{
        public int CompanyId { get; set; }
        public int NoOfMonth { get; set; }
    }
}
