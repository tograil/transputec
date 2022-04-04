namespace CrisesControl.Api.Application.Commands.Groups.GetGroup
{
    public class GetGroupResponse
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTimeOffset UpdatedOn { get; set; }
        public int CompanyId { get; set; }
        public int Status { get; set; }
    }
}
