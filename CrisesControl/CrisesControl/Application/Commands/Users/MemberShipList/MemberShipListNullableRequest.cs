using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.MemberShipList
{
    public class MemberShipListNullableRequest: IRequest<MemberShipListResponse>
    {
        public int? Start { get; set; }
        public int? Length { get; set; }
        public string? Action { get; set; }
        public string? LinkType { get; set; }
        public string? search { get; set; }        
        public List<Order>? order { get; set; }
        public string? CompanyKey { get; set; }
        public int Draw { get; set; }
    }
}
