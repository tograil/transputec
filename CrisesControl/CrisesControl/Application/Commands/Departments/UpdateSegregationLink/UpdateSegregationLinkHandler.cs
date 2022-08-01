using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.UpdateSegregationLink
{
    public class UpdateSegregationLinkHandler : IRequestHandler<UpdateSegregationLinkRequest, UpdateSegregationLinkResponse>
    {
        private readonly IDepartmentQuery _departmentQuery;
        private readonly ILogger<UpdateSegregationLinkHandler> _logger;
        public UpdateSegregationLinkHandler(IDepartmentQuery departmentQuery, ILogger<UpdateSegregationLinkHandler> logger)
        {
            this._logger = logger;
            this._departmentQuery = departmentQuery;
        }
        public async Task<UpdateSegregationLinkResponse> Handle(UpdateSegregationLinkRequest request, CancellationToken cancellationToken)
        {
             var result = await _departmentQuery.UpdateSegregationLink(request);
             return result;
            
        }
    }
}
