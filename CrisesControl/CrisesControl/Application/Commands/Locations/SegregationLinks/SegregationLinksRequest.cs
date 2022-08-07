using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.SegregationLinks
{
    public class SegregationLinksRequest:IRequest<SegregationLinksResponse>
    {
        public int TargetID { get; set; }
        public string MemberShipType { get; set; }
        public string LinkType { get; set; }
        
    }
}
