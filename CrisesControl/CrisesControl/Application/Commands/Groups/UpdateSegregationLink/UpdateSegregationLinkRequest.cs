using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.UpdateSegregationLink
{
    public class UpdateSegregationLinkRequest : IRequest<UpdateSegregationLinkResponse>
    {
        public int SourceId { get; set; }
        public int TargetId { get; set; }
        public string Action { get; set; }
        public string LinkType { get; set; }
        public int CurrentUserId { get; set; }
        public int CompanyId { get; set; }

    }
}
