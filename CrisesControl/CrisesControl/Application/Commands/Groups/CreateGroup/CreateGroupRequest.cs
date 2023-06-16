using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.CreateGroup
{
    public class CreateGroupRequest : IRequest<CreateGroupResponse>
    {
        public int CompanyId { get; set; }
        public string GroupName { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int Status { get; set; }

    }
}
