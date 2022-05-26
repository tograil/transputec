using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.Api.Application.Commands.Users.MemberShipList
{
    public class MembershipListRequestRoute
    {
        public int TargetID { get; set; }
        public int ObjMapID { get; set; }
        public MemberShipType MemberShipType { get; set; }
        public bool ActiveOnly { get; set; }
    }
}
