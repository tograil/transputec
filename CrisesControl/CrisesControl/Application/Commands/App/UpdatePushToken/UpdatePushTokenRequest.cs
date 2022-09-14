using MediatR;

namespace CrisesControl.Api.Application.Commands.App.UpdatePushToken
{
    public class UpdatePushTokenRequest:IRequest<UpdatePushTokenResponse>
    {
        public int UserDeviceId { get; set; }
        public string PushDeviceId { get; set; }
    }
}
