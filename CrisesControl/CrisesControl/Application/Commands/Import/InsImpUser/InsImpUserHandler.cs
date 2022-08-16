using Ardalis.GuardClauses;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.InsImpUser
{
    public class InsImpUserHandler : IRequestHandler<InsImpUserRequest, InsImpUserResponse>
    {
        private readonly IImportRepository _importRepository;
        public InsImpUserHandler(IImportRepository importRepository)
        {
            _importRepository = importRepository;

        }

        public async Task<InsImpUserResponse> Handle(InsImpUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(InsImpUserRequest));
            var response = new InsImpUserResponse();
            response.Result = _importRepository.CreateTempUsers(request.UserData, request.SessionId, request.CompanyId, request.JobType, request.UserId, request.TimeZoneId);
            return response;
        }
    }
}
