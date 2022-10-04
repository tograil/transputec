using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Commands.Import.GetCountActionCheck;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.InsImpDept
{
    public class InsImpDeptHandler : IRequestHandler<InsImpDeptRequest, InsImpDeptResponse>
    {
        private readonly IImportRepository _importRepository;
        public InsImpDeptHandler(IImportRepository importRepository)
        {
            _importRepository = importRepository;

        }

        public async Task<InsImpDeptResponse> Handle(InsImpDeptRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(InsImpDeptRequest));
            var response = new InsImpDeptResponse();
            response.Result =await _importRepository.CreateTempDepartment(request.Data, request.SessionId, request.CompanyId, request.UserId, request.TimeZoneId);
            return response;
        }
    }
}
