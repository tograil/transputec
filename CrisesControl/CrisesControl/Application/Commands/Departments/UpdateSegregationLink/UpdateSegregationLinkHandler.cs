using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.UpdateSegregationLink
{
    public class UpdateSegregationLinkHandler : IRequestHandler<UpdateSegregationLinkRequest, UpdateSegregationLinkResponse>
    {
        private readonly IDepartmentQuery _departmentQuery;
        private readonly ILogger<UpdateSegregationLinkHandler> _logger;
        private readonly UpdateSegregationLinkValidator _departmentValidator;
        public UpdateSegregationLinkHandler(IDepartmentQuery departmentQuery, ILogger<UpdateSegregationLinkHandler> logger, UpdateSegregationLinkValidator departmetValidator)
        {
            this._logger = logger;
            this._departmentQuery = departmentQuery;
            this._departmentValidator = departmetValidator;
        }
        public async Task<UpdateSegregationLinkResponse> Handle(UpdateSegregationLinkRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateSegregationLinkRequest));
            await _departmentValidator.ValidateAsync(request, cancellationToken);
            var result = await _departmentQuery.UpdateSegregationLink(request);
             return result;
            
        }
    }
}
