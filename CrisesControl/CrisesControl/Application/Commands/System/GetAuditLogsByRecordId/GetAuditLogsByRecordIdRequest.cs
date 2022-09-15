using MediatR;

namespace CrisesControl.Api.Application.Commands.System.GetAuditLogsByRecordId
{
    public class GetAuditLogsByRecordIdRequest:IRequest<GetAuditLogsByRecordIdResponse>
    {
        public int RecordId { get; set; }
        public string TableName { get; set; }

        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
        public bool LimitResult { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
