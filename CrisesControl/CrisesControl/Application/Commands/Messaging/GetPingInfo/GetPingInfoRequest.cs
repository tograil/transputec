using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetPingInfo
{
    public class GetPingInfoRequest : IRequest<GetPingInfoResponse>
    {
        public int MessageId { get; set; }
        //public int UserId { get; set; }
        //public int CompanyId { get; set; }
    }
}
