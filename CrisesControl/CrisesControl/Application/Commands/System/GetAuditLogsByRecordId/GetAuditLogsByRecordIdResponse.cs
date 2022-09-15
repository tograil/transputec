using CrisesControl.Core.System;

namespace CrisesControl.Api.Application.Commands.System.GetAuditLogsByRecordId
{
    public class GetAuditLogsByRecordIdResponse
    {
        public List<AuditHelp> Data { get; set; }
        public string Message { get; set; }
    }
}
