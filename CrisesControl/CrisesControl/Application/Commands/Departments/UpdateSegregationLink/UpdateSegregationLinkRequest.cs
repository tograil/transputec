using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.UpdateSegregationLink
{
    public class UpdateSegregationLinkRequest:IRequest<UpdateSegregationLinkResponse>
    {
        public int SourceId { get; set; }
        public int TargetId { get; set; }
         public GroupType LinkType { get; set; }
    }
}
