using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskUserList
{
    public class GetTaskUserListResponse
    {
        public DataTablePaging tablePaging { get; set; }
        public string Message { get; set; }
    }
}
