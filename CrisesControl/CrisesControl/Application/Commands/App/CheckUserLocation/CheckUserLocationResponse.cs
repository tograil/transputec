using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.App.CheckUserLocation
{
    public class CheckUserLocationResponse
    {
        public List<UserLocation> Result { get; set; }
        public string Message { get; set; }
    }
}
