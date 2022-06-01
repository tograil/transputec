namespace CrisesControl.Api.Application.Commands.Users.MembershipList
{
    public class MembershipResponse
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public object data { get; set; }
    }
}
