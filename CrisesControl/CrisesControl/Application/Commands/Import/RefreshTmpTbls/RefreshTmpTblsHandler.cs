using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.RefreshTmpTbls
{
    public class RefreshTmpTblsHandler : IRequestHandler<RefreshTmpTblsRequest, RefreshTmpTblsResponse>
    {
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;
        public RefreshTmpTblsHandler(IImportRepository importRepository, IMapper mapper)
        {
            _importRepository = importRepository;
            _mapper = mapper;

        }

        public async Task<RefreshTmpTblsResponse> Handle(RefreshTmpTblsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(RefreshTmpTblsRequest));
            var result = await _importRepository.RefreshTmpTable(request.CompanyId, request.UserId, request.SessionId);
            var response = _mapper.Map<RefreshTmpTblsResponse>(result);
            return response;
        }
    }
}
