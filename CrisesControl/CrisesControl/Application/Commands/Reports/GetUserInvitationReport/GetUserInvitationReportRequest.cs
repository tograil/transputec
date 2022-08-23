using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserInvitationReport
{
    public class GetUserInvitationReportRequest : CcBase, IRequest<GetUserInvitationReportResponse>
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
    }
}
