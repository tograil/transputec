using CrisesControl.Core.Import;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.RefreshTmpTbls
{
    public class RefreshTmpTblsRequest : IRequest<RefreshTmpTblsResponse>
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string SessionId { get; set; }
    }
}
