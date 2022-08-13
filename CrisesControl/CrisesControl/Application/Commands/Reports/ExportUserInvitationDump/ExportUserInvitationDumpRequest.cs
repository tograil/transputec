using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.ExportUserInvitationDump
{
    public class ExportUserInvitationDumpRequest : CcBase, IRequest<ExportUserInvitationDumpResponse>
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
    }
}
