using CrisesControl.Core.App;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.CaptureUserLocation
{
    public class CaptureUserLocationRequest:IRequest<CaptureUserLocationResponse>
    {
        public List<LocationInfo> UserLocations { get; set; }
        public int UserDeviceId { get; set; }
    }
}
