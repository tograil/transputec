using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartments
{
    public class GetDepartmentsHandler:IRequestHandler<GetDepartmentsRequest,GetDepartmentsResponse>
    {
        private readonly ILogger<GetDepartmentsHandler> _logger;
        private readonly GetDepartmentsValidator _departmentsValidator;
        private readonly IDepartmentQuery _departmentQuery;
        public GetDepartmentsHandler(GetDepartmentsValidator departmentsValidator, IDepartmentQuery departmentQuery, ILogger<GetDepartmentsHandler> logger)
        {
            _departmentsValidator = departmentsValidator;
            _departmentQuery = departmentQuery;
            _logger = logger;
        }

        public async Task<GetDepartmentsResponse> Handle(GetDepartmentsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetDepartmentsRequest));
            await _departmentsValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _departmentQuery.GetDepartments(request, cancellationToken);
            return result;
        }
    }
}
