using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Administrator.GetReport
{
    public class GetReportResponse
    {
        public AdminReport Data { get; set; }
        public string Message { get; set; }
    }
}
