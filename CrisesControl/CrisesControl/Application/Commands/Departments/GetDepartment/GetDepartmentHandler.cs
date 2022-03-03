using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.GetDepartment
{
    public class GetDepartmentHandler: IRequestHandler<GetDepartmentRequest, GetDepartmentResponse>
    {
        private readonly GetDepartmentValidator _departmentValidator;
        private readonly IDepartmentQuery _departmentQuery;

        public GetDepartmentHandler(GetDepartmentValidator departmentValidator, IDepartmentQuery departmentQuery)
        {
            _departmentValidator = departmentValidator;
            _departmentQuery = departmentQuery;
        }

        public async Task<GetDepartmentResponse> Handle(GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetDepartmentRequest));
            
            await _departmentValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var departments = await _departmentQuery.GetDepartment(request);

            return new GetDepartmentResponse();
        }
    }
}
