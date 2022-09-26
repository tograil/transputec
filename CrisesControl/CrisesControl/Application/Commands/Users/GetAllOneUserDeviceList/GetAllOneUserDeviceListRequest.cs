using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetAllOneUserDeviceList
{
    public class GetAllOneUserDeviceListRequest:IRequest<GetAllOneUserDeviceListResponse>
    {
        public int QueriedUserId { get; set; }
        public string Filters { get; set; }
    }
}
