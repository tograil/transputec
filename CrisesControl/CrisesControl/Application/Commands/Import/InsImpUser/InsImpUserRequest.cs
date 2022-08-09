using CrisesControl.Core.Import;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.InsImpUser
{
    public class InsImpUserRequest : IRequest<InsImpUserResponse>
    {
        public List<ImportDumpInput> UserData { get; set; }
        public string SessionId { get; set; }
        public int CompanyId { get; set; }
        public string JobType { get; set; }
        public int UserId { get; set; }
        public string TimeZoneId { get; set; }
    }
}
