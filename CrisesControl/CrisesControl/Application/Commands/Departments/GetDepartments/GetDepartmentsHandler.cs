using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Departments.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartments
{
    public class GetDepartmentsHandler : IRequestHandler<GetDepartmentsRequest,GetDepartmentsResponse>
    {
        private readonly GetDepartmentsValidator _departmentValidator;
        private readonly IDepartmentQuery _departmentQuery;

        public GetDepartmentsHandler(GetDepartmentsValidator departmentValidator, 
            IDepartmentRepository departmentService,
            IDepartmentQuery departmentQuery)
        {
            _departmentValidator = departmentValidator;
            _departmentQuery = departmentQuery;
        }

        public async Task<GetDepartmentsResponse> Handle(GetDepartmentsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetDepartmentsRequest));
            
            await _departmentValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var departments = await _departmentQuery.GetDepartments(request);
            return departments;
        }
    }
}
