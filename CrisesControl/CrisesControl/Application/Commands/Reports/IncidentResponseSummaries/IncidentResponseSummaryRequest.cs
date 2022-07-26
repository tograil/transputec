using CrisesControl.Api.Application.Commands.Reports.IncidentResponseSummaries;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.IncidentResponseSummary
{
    public class IncidentResponseSummaryRequest:IRequest<IncidentResponseSummaryResponse>
    {
        public int ActiveIncidentID { get; set; }
    }
}
