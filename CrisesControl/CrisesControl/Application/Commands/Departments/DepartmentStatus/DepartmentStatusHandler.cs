using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Departments.DepartmentStatus
{
    public class DepartmentStatusHandler : IRequestHandler<DepartmentStatusRequest, DepartmentStatusResponse>
    {
        private readonly IDepartmentQuery _departmentQuery;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentStatusHandler> _logger;
        private readonly DepartmentStatusValidator _departmentStatusValidator;
        public DepartmentStatusHandler(IMapper mapper, IDepartmentQuery departmentQuery, ILogger<DepartmentStatusHandler> logger, DepartmentStatusValidator departmentStatusValidator)
        {
            this._departmentQuery = departmentQuery;
            this._mapper = mapper;
            this._logger = logger;
            this._departmentStatusValidator = departmentStatusValidator;
        }
        public async Task<DepartmentStatusResponse> Handle(DepartmentStatusRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DepartmentStatusRequest));
            await _departmentStatusValidator.ValidateAsync(request,cancellationToken);
            var result = await _departmentQuery.DepartmentStatus(request);
            return result;
        }
    }
}
