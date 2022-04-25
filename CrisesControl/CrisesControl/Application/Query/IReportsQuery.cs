using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;

namespace CrisesControl.Api.Application.Query {
    public interface IReportsQuery {
        public Task<GetSOSItemsResponse> GetSOSItems(GetSOSItemsRequest request);
    }
}
