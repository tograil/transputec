using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTempDept
{
    public class GetTempDeptResponse
    {
        public UserDepartment Data { get; set; }
        public string Message { get; set; }
    }
}
