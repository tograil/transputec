using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Departments;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.DeleteDepartment
{
    public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentRequest, DeleteDepartmentResponse>
    {
        private readonly DeleteDepartmentValidator _departmentValidator;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public DeleteDepartmentHandler(DeleteDepartmentValidator departmentValidator, IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentValidator = departmentValidator;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }
        public async Task<DeleteDepartmentResponse> Handle(DeleteDepartmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteDepartmentRequest));

            Department value = _mapper.Map<DeleteDepartmentRequest, Department>(request);
            if (!CheckForExistance(value.DepartmentId))
            {
                var departmentId = await _departmentRepository.DeleteDepartment(value.DepartmentId, cancellationToken);
                var result = new DeleteDepartmentResponse();
                result.DepartmentId = departmentId;
                return result;
            }
            return null;
        }

        private bool CheckForExistance(int departmentId)
        {
            return _departmentRepository.CheckForExistance(departmentId);
        }
    }
}
