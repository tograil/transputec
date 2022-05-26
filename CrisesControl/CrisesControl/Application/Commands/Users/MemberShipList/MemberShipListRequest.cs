using CrisesControl.Core.Compatibility;
using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.MemberShipList
{
    public class MemberShipListRequest: IRequest<MemberShipListResponse>
    {
        public int TargetID { get; set; }
        public int ObjMapID { get; set; }
        public MemberShipType MemberShipType { get; set; }
        public bool ActiveOnly { get; set; }
        public int? Start { get; set; }
        public int? Length { get; set; }
        public string? Action { get; set; }
        public string? search { get; set; }
        public List<Order>? order { get; set; }
        public string? CompanyKey { get; set; }
        public int Draw { get; set; }

    }
}
