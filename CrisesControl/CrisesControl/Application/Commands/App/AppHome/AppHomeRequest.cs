using MediatR;

namespace CrisesControl.Api.Application.Commands.App.AppHome
{
    public class AppHomeRequest:IRequest<AppHomeResponse>
    {
        public int UserDeviceID { get; set; }
        public string Token { get; set; }
    }
}
