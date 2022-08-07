using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetMonthlyTransaction
{
    public class GetMonthlyTransactionHandler : IRequestHandler<GetMonthlyTransactionRequest, GetMonthlyTransactionResponse>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<GetMonthlyTransactionHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public GetMonthlyTransactionHandler(IAdminRepository adminRepository, ILogger<GetMonthlyTransactionHandler> logger, IMapper mapper)
        {
            this._adminRepository = adminRepository;
            this._logger = logger;
            this._mapper = mapper;
        }
        public async Task<GetMonthlyTransactionResponse> Handle(GetMonthlyTransactionRequest request, CancellationToken cancellationToken)
        {
            var transactions = await _adminRepository.GetMonthlyTransaction(_currentUser.CompanyId);
            var result = _mapper.Map<List<AdminTransaction>>(transactions);
            var response = new GetMonthlyTransactionResponse();
            if (result != null)
            {
                response.Data = transactions;
            }
            else
            {
                response.Data = new List<AdminTransaction>();
            }
            return response;
        }
    }
}
