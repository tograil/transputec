using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartment
{
    public class GetDepartmentHandler : IRequestHandler<GetDepartmentRequest, GetDepartmentResponse>
    {

        private readonly ILogger<GetDepartmentHandler> _logger;
        private readonly GetDepartmentValidator _departmentValidator;
        private readonly IDepartmentQuery _departmentQuery;
        public GetDepartmentHandler(GetDepartmentValidator departmentValidator, IDepartmentQuery departmentQuery, ILogger<GetDepartmentHandler> logger)
        {
            _departmentValidator = departmentValidator;
            _departmentQuery = departmentQuery;
            _logger = logger;
        }
        public async Task<GetDepartmentResponse> Handle(GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetDepartmentRequest));

            await _departmentValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _departmentQuery.GetDepartment(request, cancellationToken);
            return result;
        }
    }
}
