using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetUsageGraph
{
    public class GetUsageGraphRequest : IRequest<GetUsageGraphResponse>
    {
        public string ReportType { get; set; }
        public int LastMonth { get; set; }
    }
}
