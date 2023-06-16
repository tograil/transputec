using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Lookup.GetAllTmpLoc
{
    public class GetAllTmpLocResponse
    {
        public List<UserLocation> Data { get; set; }
        public string Message { get; set; }
    }
}
