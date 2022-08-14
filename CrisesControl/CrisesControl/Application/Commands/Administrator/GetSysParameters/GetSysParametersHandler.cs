using AutoMapper;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetSysParameters
{
    public class GetSysParametersHandler : IRequestHandler<GetSysParametersRequest, GetSysParametersResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<GetSysParametersHandler> _logger;
        private readonly IMapper _mapper;
        public GetSysParametersHandler(IAdminRepository adminRepository, ILogger<GetSysParametersHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
        }
        public async Task<GetSysParametersResponse> Handle(GetSysParametersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var libIncidents = await _adminRepository.GetSysParameters(request.SystemParameterId);
                var result = _mapper.Map<List<SysParameter>>(libIncidents);
                var response = new GetSysParametersResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = null;
                    response.Message = "No record found.";
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
