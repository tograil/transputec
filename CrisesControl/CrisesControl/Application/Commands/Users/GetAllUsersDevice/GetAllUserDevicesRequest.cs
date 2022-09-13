using CrisesControl.Core.Users;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetAllUsersDevice
{
    public class GetAllUserDevicesRequest:IRequest<GetAllUserDevicesResponse>
    {
        public GetAllUserDeviceRequest DeviceRequest { get; set; }
    }
}
