using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Departments.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.CreateDepartment
{
    public class CreateDepartmentHandler: IRequestHandler<CreateDepartmentRequest, CreateDepartmentResponse>
    {
        private readonly CreateDepartmentValidator _departmentValidator;
        private readonly IDepartmentQuery _departmentQuery;
        private readonly ILogger<CreateDepartmentHandler> _logger;

        public CreateDepartmentHandler(CreateDepartmentValidator departmentValidator, IDepartmentQuery departmentQuery, ILogger<CreateDepartmentHandler> logger)
        {
            _departmentValidator = departmentValidator;
            _departmentQuery = departmentQuery;
            _logger = logger;
        }

        public async Task<CreateDepartmentResponse> Handle(CreateDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateDepartmentRequest));
            await _departmentValidator.ValidateAndThrowAsync(request, cancellationToken);
           var result = await _departmentQuery.CreateDepartment(request, cancellationToken);
                 return result;
          
        }

    }
}
