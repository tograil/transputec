using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.Departments.UpdateSegregationLink;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Departments;
using CrisesControl.Core.Departments.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.UpdateDepartment
{
    public class UpdateDepartmentHandler: IRequestHandler<UpdateDepartmentRequest, UpdateDepartmentResponse>
    {
        private readonly UpdateDepartmentValidator _departmentValidator;
        private readonly IDepartmentQuery _departmentQuery;
        private readonly ILogger<UpdateDepartmentHandler> _logger;

        public UpdateDepartmentHandler(UpdateDepartmentValidator departmentValidator, IDepartmentQuery departmentQuery, ILogger<UpdateDepartmentHandler> logger)
        {
            _departmentValidator = departmentValidator;
            _departmentQuery = departmentQuery;
            _logger = logger;
        }

        public async Task<UpdateDepartmentResponse> Handle(UpdateDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateDepartmentRequest));
            await _departmentValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _departmentQuery.UpdateDepartment(request, cancellationToken);
            return result;
          
        }

    }
}
