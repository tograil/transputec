using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ResetUserDeviceToken
{
    public class ResetUserDeviceTokenRequest:IRequest<ResetUserDeviceTokenResponse>
    {
        public int UserId { get; set; }
    }
}
