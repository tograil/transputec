using AutoMapper;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetAllSysParameters
{
    public class GetAllSysParametersHandler : IRequestHandler<GetAllSysParametersRequest, GetAllSysParametersResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<GetAllSysParametersHandler> _logger;
        private readonly IMapper _mapper;
        public GetAllSysParametersHandler(IAdminRepository adminRepository, ILogger<GetAllSysParametersHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
        }
        public async Task<GetAllSysParametersResponse> Handle(GetAllSysParametersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var sysParameters = await _adminRepository.GetAllSysParameters();
                var result = _mapper.Map<SysParameter>(sysParameters);
                var response = new GetAllSysParametersResponse();
                if (result != null)
                {
                    response.Data = result;                    
                }
                else
                {
                    response.Data = null;                  
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
