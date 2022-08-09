using Ardalis.GuardClauses;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.InsImpGroup
{
    public class InsImpGroupHandler : IRequestHandler<InsImpGroupRequest, InsImpGroupResponse>
    {
        private readonly IImportRepository _importRepository;
        public InsImpGroupHandler(IImportRepository importRepository)
        {
            _importRepository = importRepository;

        }

        public async Task<InsImpGroupResponse> Handle(InsImpGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(InsImpGroupRequest));
            var response = new InsImpGroupResponse();
            response = _importRepository.CreateTempGroup(request);
            return response;
        }
    }
}
