using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.SegregationLinks
{
    public class SegregationLinksRequest: IRequest<SegregationLinksResponse>
    {
      public  int TargetID { get; set; }
      public  MemberShipType MemberShipType{ get; set; }
      public string LinkType { get; set; }
 
    }
}
