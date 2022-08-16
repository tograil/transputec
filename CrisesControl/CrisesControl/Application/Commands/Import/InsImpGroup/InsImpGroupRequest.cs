using CrisesControl.Core.Import;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.InsImpGroup
{
    public class InsImpGroupRequest : IRequest<InsImpGroupResponse>
    {
        public List<ImportDumpInput> Data { get; set; }
        public string SessionId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string TimeZoneId { get; set; }
    }
}
