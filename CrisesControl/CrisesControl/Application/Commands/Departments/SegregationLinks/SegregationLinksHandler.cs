using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.SegregationLinks
{
    public class SegregationLinksHandler : IRequestHandler<SegregationLinksRequest, SegregationLinksResponse>
    {
        private readonly ILogger<SegregationLinksHandler> _logger;
        private readonly IDepartmentQuery _departmentQuery;
        
        public SegregationLinksHandler(ILogger<SegregationLinksHandler> logger, IDepartmentQuery departmentQuery)
        {
            this._departmentQuery = departmentQuery;
            this._logger = logger;
        }
        public async Task<SegregationLinksResponse> Handle(SegregationLinksRequest request, CancellationToken cancellationToken)
        {
            var result = await _departmentQuery.SegregationLinks(request);
            return result;
            
        }
    }
}
