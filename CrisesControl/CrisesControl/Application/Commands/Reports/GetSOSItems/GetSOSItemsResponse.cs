using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetSOSItems {
    public class GetSOSItemsResponse {
        public List<SOSItem> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
