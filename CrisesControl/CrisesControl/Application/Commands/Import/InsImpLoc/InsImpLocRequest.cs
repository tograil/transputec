using CrisesControl.Core.Import;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.InsImpLoc
{
    public class InsImpLocRequest: IRequest<InsImpLocResponse>
    {
        public List<ImportDumpInput> LocData { get; set; }
        public string SessionId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string TimeZoneId { get; set; }
    }
}
