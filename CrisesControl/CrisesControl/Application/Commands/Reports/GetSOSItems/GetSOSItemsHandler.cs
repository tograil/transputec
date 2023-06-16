using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetSOSItems {
    public class GetSOSItemsHandler : IRequestHandler<GetSOSItemsRequest, GetSOSItemsResponse> {

        private readonly IReportsQuery _reportsQuery;

        public GetSOSItemsHandler(IReportsQuery reportsQuery) {
            _reportsQuery = reportsQuery;
        }

        public async Task<GetSOSItemsResponse> Handle(GetSOSItemsRequest request, CancellationToken cancellationToken) {
            var result = await _reportsQuery.GetSOSItems(request);
            return result;
        }
    }
}
