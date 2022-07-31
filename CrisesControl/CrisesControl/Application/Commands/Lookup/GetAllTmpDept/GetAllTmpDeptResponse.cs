using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Lookup.GetAllTmpDept
{
    public class GetAllTmpDeptResponse
    {
        public List<UserDepartment> Data { get; set; }
        public string Message { get; set; }
    }
}
