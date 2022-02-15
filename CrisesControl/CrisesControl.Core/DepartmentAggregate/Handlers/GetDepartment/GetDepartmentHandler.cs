using Ardalis.GuardClauses;
using CrisesControl.Core.DepartmentAggregate.Services;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.DepartmentAggregate.Handles.GetDepartment
{
    public class GetDepartmentHandler: IRequestHandler<GetDepartmentRequest, GetDepartmentResponse>
    {
        private readonly GetDepartmentValidator _departmentValidator;
        private readonly IDepartmentService _departmentService;

        public GetDepartmentHandler(GetDepartmentValidator departmentValidator, IDepartmentService departmentService)
        {
            _departmentValidator = departmentValidator;
            _departmentService = departmentService;
        }

        public async Task<GetDepartmentResponse> Handle(GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetDepartmentRequest));
            
            await _departmentValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var departments = await _departmentService.GetAllDepartments();

            return new GetDepartmentResponse();
        }
    }
}
