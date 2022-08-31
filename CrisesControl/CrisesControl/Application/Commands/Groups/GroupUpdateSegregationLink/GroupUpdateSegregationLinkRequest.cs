using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GroupUpdateSegregationLink
{
    public class GroupUpdateSegregationLinkRequest : IRequest<GroupUpdateSegregationLinkResponse>
    {
        public int SourceId { get; set; }
        public int TargetId { get; set; }
        public string Action { get; set; }
        public string LinkType { get; set; }
        public int CurrentUserId { get; set; }
        public int CompanyId { get; set; }

    }
}
