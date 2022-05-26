namespace CrisesControl.Api.Application.Commands.Users.MemberShipList
{
    public class MemberShipListResponse
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public object data { get; set; }
    }
}
