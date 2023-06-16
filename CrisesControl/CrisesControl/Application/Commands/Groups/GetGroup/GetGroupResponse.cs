using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.Groups.GetGroup
{
    public class GetGroupResponse
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int Status { get; set; }
        public int ObjectMappingId { get; set; }
        public int UserCount { get; set; }
        public int ActiveUserCount { get; set; }
        public List<User> UserList { get; set; }
        public UserFullName CreatedByName { get; set; }
        public UserFullName UpdatedByName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CompanyId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
