using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.Login
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        public string IPAddress { get; set; }
        public string Language { get; set; }
        public string DeviceType { get; set; }
    }
}
