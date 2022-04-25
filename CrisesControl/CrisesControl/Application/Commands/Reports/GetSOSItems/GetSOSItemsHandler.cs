using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetSOSItems {
    public class GetSOSItemsHandler : IRequestHandler<GetSOSItemsRequest, GetSOSItemsResponse> {

        private readonly IReportsQuery _reportsQuery;
        private readonly IMapper _mapper;

        public GetSOSItemsHandler(IReportsQuery reportsQuery, IMapper mapper) {
            _reportsQuery = reportsQuery;
            _mapper = mapper;
        }

        public async Task<GetSOSItemsResponse> Handle(GetSOSItemsRequest request, CancellationToken cancellationToken) {
            var result = await _reportsQuery.GetSOSItems(request);
            return result;
        }
    }
}
