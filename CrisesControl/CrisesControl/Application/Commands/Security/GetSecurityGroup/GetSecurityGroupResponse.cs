using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Security.GetSecurityGroup
{
    public class GetSecurityGroupResponse
    {
        public SecurityGroup Data { get; set; }
        public string Message { get; set; }
    }
}
