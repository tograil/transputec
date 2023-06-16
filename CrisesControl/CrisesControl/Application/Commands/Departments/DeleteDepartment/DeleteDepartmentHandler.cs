using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Departments;
using MediatR;
using CrisesControl.Api.Application.Query;
using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Departments.DeleteDepartment
{
    public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentRequest, DeleteDepartmentResponse>
    {
        private readonly DeleteDepartmentValidator _departmentValidator;
        private readonly IDepartmentQuery _departmentQuery;
        private readonly ILogger<DeleteDepartmentHandler> _logger;

        public DeleteDepartmentHandler(DeleteDepartmentValidator departmentValidator, IDepartmentQuery departmentQuery, ILogger<DeleteDepartmentHandler> logger)
        {
            _departmentValidator = departmentValidator;
            _departmentQuery = departmentQuery;
            _logger = logger;
        }
        public async Task<DeleteDepartmentResponse> Handle(DeleteDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteDepartmentRequest));
            await _departmentValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result =await _departmentQuery.DeleteDepartment(request,cancellationToken);
            return result;
        }

       
    }
}
