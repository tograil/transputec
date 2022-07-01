using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetTaskPerformance
{
    public class GetTaskPerformanceResponse
    {
        public TaskPerformance Data { get; set; }
        public FailedTaskReport failedTask { get; set; }
        public FailedTaskReport CompleteFailed { get; set; }
        public string Message { get; set; }

    }
}
