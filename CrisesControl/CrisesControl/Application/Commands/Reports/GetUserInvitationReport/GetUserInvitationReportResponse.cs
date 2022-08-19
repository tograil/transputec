using CrisesControl.Core.Import;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserInvitationReport
{
    public class GetUserInvitationReportResponse : CommonDTO
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public object Data { get; set; }
    }
}
