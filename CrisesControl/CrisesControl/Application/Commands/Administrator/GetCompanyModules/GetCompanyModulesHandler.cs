using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyModules
{
    public class GetCompanyModulesHandler : IRequestHandler<GetCompanyModulesRequest, GetCompanyModulesResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<GetCompanyModulesHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public GetCompanyModulesHandler(IAdminRepository adminRepository, ILogger<GetCompanyModulesHandler> logger, IMapper mapper, ICurrentUser currentUser)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
            this._currentUser = currentUser;
        }
        public async Task<GetCompanyModulesResponse> Handle(GetCompanyModulesRequest request, CancellationToken cancellationToken)
        {
            var transactions = await _adminRepository.GetPackageAddons(_currentUser.CompanyId, true);
            var result = _mapper.Map<List<PackageAddons>>(transactions);
            var response = new GetCompanyModulesResponse();
            if (result != null)
            {
                response.data = transactions;
            }
            else
            {
                response.data = new List<PackageAddons>();
            }
            return response;
        }
    }
}
