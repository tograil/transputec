using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.DepartmentOnlyImport
{
    public class DepartmentOnlyImportRequest : IRequest<DepartmentOnlyImportResponse>
    {
        public string SessionId { get; set; }
        public List<GroupData> DeptData { get; set; }

    }

    public class GroupData
    {
        public GroupData()
        {
            Action = "ADD";
            Status = 0;
        }
        public string GroupName { get; set; }
        public int Status { get; set; }
        public string Action { get; set; }
    }
}
