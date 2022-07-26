using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetFailedAttempts
{
    public class GetFailedAttemptsResponse
    {
        public List<FailedAttempts> Data { get; set; }
        public string Message { get; set; }
    }
}
