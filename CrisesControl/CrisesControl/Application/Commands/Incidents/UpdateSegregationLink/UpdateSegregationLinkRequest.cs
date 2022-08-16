using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.UpdateSegregationLink
{
    public class UpdateSegregationLinkRequest:IRequest<UpdateSegregationLinkResponse>
    {
        public int SourceID { get; set; }
        public int TargetID { get; set; }
        public GroupType LinkType { get; set; }
    }
}
