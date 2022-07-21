using CrisesControl.Core.Security;

namespace CrisesControl.Api.Application.Commands.Security.GetAllSecurityObjects
{
    public class GetAllSecurityObjectsResponse
    {
        public List<SecurityAllObjects> Data { get; set; }
        public string Message { get; set; }
    }
}
