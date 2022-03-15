using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.UpdateGroup
{
    public class UpdateGroupRequest : IRequest<UpdateGroupResponse>
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int CompanyId { get; set; }
        public int Status { get; set; }
    }
}
