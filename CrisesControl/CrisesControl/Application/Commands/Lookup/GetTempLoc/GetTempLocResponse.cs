using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTempLoc
{
    public class GetTempLocResponse
    {
        public UserLocation Data { get; set; }
        public string Message { get; set; }
    }
}
