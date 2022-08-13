using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.SegregationLinks
{
    public class SegregationLinksRequest:IRequest<SegregationLinksResponse>
    {
        public string MemberShipType { get; set; }
        public int TargetID { get; set; }
        public string LinkType { get; set; }
        public int OutUserCompanyId { get; set; }
    }
}
