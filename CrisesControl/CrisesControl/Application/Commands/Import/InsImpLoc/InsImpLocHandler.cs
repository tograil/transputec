using Ardalis.GuardClauses;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.InsImpLoc
{
    public class InsImpLocHandler : IRequestHandler<InsImpLocRequest, InsImpLocResponse>
    {
        private readonly IImportRepository _importRepository;
        public InsImpLocHandler(IImportRepository importRepository)
        {
            _importRepository = importRepository;

        }

        public async Task<InsImpLocResponse> Handle(InsImpLocRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(InsImpLocRequest));
            var response = new InsImpLocResponse();
            response.Result =await _importRepository.CreateTempLocation(request.LocData, request.SessionId, request.CompanyId, request.UserId, request.TimeZoneId);
            return response;
        }
    }
}
