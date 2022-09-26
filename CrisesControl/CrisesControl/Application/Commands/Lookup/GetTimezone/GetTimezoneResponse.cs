using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTimezone
{
    public class GetTimezoneResponse
    {
        public List<StdTimeZone> Data { get; set; }
    }
}
