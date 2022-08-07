using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdateSysParameters
{
    public class UpdateSysParametersHandler : IRequestHandler<UpdateSysParametersRequest, UpdateSysParametersResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<UpdateSysParametersHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public UpdateSysParametersHandler(IAdminRepository adminRepository, ICurrentUser currentUser, ILogger<UpdateSysParametersHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
            this._currentUser = currentUser;
        }
        public async Task<UpdateSysParametersResponse> Handle(UpdateSysParametersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var sysParameter = await _adminRepository.UpdateSysParameters(request.SysParametersId, request.Category, request.Name, request.Value, request.Type, request.Display, request.Description,
                        request.Status, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<int>(sysParameter);
                var response = new UpdateSysParametersResponse();
                if (result > 0)
                {
                    response.result = sysParameter;
                }
                else
                {
                    response.result = 0;
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
