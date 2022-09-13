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
        private readonly IMapper _mapper;

        public DeleteDepartmentHandler(DeleteDepartmentValidator departmentValidator, IDepartmentQuery departmentQuery, IMapper mapper)
        {
            _departmentValidator = departmentValidator;
            _departmentQuery = departmentQuery;
            _mapper = mapper;
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
