using Ardalis.GuardClauses;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.GetCountActionCheck
{
    public class GetCountActionCheckHandler : IRequestHandler<GetCountActionCheckRequest, GetCountActionCheckResponse>
    {
        private readonly IImportRepository _importRepository;
        public GetCountActionCheckHandler(IImportRepository importRepository)
        {
            _importRepository = importRepository;

        }

        public async Task<GetCountActionCheckResponse> Handle(GetCountActionCheckRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCountActionCheckRequest));
            var response = new GetCountActionCheckResponse();
            response = _importRepository.GetCountActionCheck(request);
            return response;
        }
    }
}
