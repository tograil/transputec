using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Import;
using CrisesControl.Core.Import.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.UserCompleteImport
{
    public class UserCompleteImportHandler : IRequestHandler<UserCompleteImportRequest, UserCompleteImportResponse>
    {
        private readonly UserCompleteImportValidator _importValidator;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public UserCompleteImportHandler(UserCompleteImportValidator importValidator, IImportRepository importRepository, IMapper mapper)
        {
            _importValidator = importValidator;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<UserCompleteImportResponse> Handle(UserCompleteImportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UserCompleteImportRequest));

            UserCompleteImportModel value = _mapper.Map<UserCompleteImportRequest, UserCompleteImportModel>(request);
            _importRepository.UserCompleteImport(value, cancellationToken);
            return null;
        }
    }
}
