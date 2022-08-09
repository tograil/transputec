using CrisesControl.Core.Import;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.InsImpDept
{
    public class InsImpDeptRequest : IRequest<InsImpDeptResponse>
    {
        public List<ImportDumpInput> Data { get; set; }
        public string SessionId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string TimeZoneId { get; set; }
    }
}
