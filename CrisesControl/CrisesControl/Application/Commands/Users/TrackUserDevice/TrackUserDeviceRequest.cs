using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.TrackUserDevice
{
    public class TrackUserDeviceRequest : IRequest<TrackUserDeviceResponse>
    {
        public int UserId { get; set; }
    }
}
