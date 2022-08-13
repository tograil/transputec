using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.AddSysParameters
{
    public class AddSysParametersHandler : IRequestHandler<AddSysParametersRequest, AddSysParametersResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AddSysParametersHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public AddSysParametersHandler(IAdminRepository adminRepository, ICurrentUser currentUser,ILogger<AddSysParametersHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
            this._currentUser = currentUser;
        }
        public async Task<AddSysParametersResponse> Handle(AddSysParametersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var sysParameter = await _adminRepository.UpdateSysParameters(0, request.Category, request.Name, request.Value, request.Type, request.Display, request.Description,
                        1,_currentUser.UserId , _currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<int>(sysParameter);
                var response = new AddSysParametersResponse();
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
