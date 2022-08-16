using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.SegregationLinks
{
    public class SegregationLinksRequest:IRequest<SegregationLinksResponse>
    {
        public int TargetID { get; set; }
        public int ObjMapID { get; set; }
        public string MemberShipType { get; set; }
        public GroupType LinkType { get; set; }
    }
}
