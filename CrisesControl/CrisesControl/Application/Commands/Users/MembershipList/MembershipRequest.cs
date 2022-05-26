using CrisesControl.Core.Compatibility;
using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.MembershipList
{
    public class MembershipRequest: IRequest<MembershipResponse>
    {
        public int TargetID { get; set; }
        public int ObjMapID { get; set; }
        public MemberShipType MemberShipType { get; set; }
        public bool ActiveOnly { get; set; }
        public string? OrderDir { get; set; }
        public string? search { get; set; }       
        public string? CompanyKey { get; set; }
        public int Draw { get; set; }
    }
}
