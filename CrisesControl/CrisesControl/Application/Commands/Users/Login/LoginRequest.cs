using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.Login
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        public string PushDeviceId { get; set; }
        public string DeviceSerial { get; set; }
        public string DeviceType { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceModel { get; set; }
    }
}
