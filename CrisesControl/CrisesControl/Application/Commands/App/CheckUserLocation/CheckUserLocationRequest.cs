using MediatR;

namespace CrisesControl.Api.Application.Commands.App.CheckUserLocation
{
    public class CheckUserLocationRequest:IRequest<CheckUserLocationResponse>
    {
        public int UserDeviceID { get; set; }
        public int Length { get; set; }
        public string Action { get; set; }
      
    }
}
