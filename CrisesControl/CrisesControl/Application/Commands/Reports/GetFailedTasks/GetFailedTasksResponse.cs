using CrisesControl.Core.Compatibility;

namespace CrisesControl.Api.Application.Commands.Reports.GetFailedTasks
{
    public class GetFailedTasksResponse
    {
      public  DataTablePaging Data { get; set; }
        public string Message { get; set; }
    }
}
