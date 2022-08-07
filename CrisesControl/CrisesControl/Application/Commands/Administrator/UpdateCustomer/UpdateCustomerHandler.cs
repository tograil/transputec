using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdateCustomer
{
    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerRequest, UpdateCustomerResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<UpdateCustomerHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public UpdateCustomerHandler(IAdminRepository adminRepository, ICurrentUser currentUser, ILogger<UpdateCustomerHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
            this._currentUser = currentUser;
        }
        public async Task<UpdateCustomerResponse> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _adminRepository.UpdateCustomerId(request.NewCustomerId,request.QCompanyId,request.QCustomerId);
                var result = _mapper.Map<int>(customer);
                var response = new UpdateCustomerResponse();
                if (result > 0)
                {
                    response.Result = customer;
                }
                else
                {
                    response.Result = 0;
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
