using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.DeleteUserDevice
{
    public class DeleteUserDeviceRequest : IRequest<DeleteUserDeviceResponse>
    {
        public int UserDeviceID { get; set; }
    }
}
