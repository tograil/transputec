using AutoMapper;
using CrisesControl.Api.Application.Commands.Register.CheckCustomer;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Register.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class RegisterQuery : IRegisterQuery
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<RegisterQuery> _logger;
        private readonly IMapper _mapper;
        public RegisterQuery(IRegisterRepository registerRepository, IMapper mapper,
        ILogger<RegisterQuery> logger)
        {
           this. _mapper = mapper;
            this._registerRepository = registerRepository;
            this._logger = logger;
        }
        public async Task<CheckCustomerResponse> CheckCustomer(CheckCustomerRequest request)
        {
            var customer = await _registerRepository.CheckCustomer(request.CustomerId);                    
            var result = _mapper.Map<CheckCustomerResponse>(customer);
            if (customer)
            {
                result.Message = "Customer ID already taken";
            }
            else
            {
                result.Message = "No record found.";
            }

            return result;
        }
    }
}
