using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserSystemInfo
{
    public class GetUserSystemInfoRequest : IRequest<GetUserSystemInfoResponse>
    {
        public int QUserId { get; set; }
        public int CompanyId { get; set; }
    }
}
