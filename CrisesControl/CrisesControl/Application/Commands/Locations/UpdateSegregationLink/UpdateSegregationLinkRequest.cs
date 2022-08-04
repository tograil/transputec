using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.UpdateSegregationLink
{
    public class UpdateSegregationLinkRequest:IRequest<UpdateSegregationLinkResponse>
    {
        public int TargetID { get; set; }
        public int SourceID { get; set; }
        public GroupType LinkType { get; set; }
        public string Action { get; set; }
    }
}
