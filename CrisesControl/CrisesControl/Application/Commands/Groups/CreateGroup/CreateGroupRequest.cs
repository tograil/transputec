using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.CreateGroup
{
    public class CreateGroupRequest : IRequest<CreateGroupResponse>
    {
        public int CompanyId { get; set; }
        public string? GroupName { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public DateTimeOffset? UpdateOn { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public int? Status { get; set; }

    }
}
