using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.CompanyParameters.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SegregationOtp
{
    public class SegregationOtpHandler : IRequestHandler<SegregationOtpRequest, SegregationOtpResponse>
    {
        private ICompanyParametersRepository _companyParametersRepository;
        private IMapper _mapper;
        public SegregationOtpHandler(ICompanyParametersRepository companyParametersRepository, IMapper mapper)
        {
            _companyParametersRepository = companyParametersRepository;
            _mapper = mapper;
        }

        public async Task<SegregationOtpResponse> Handle(SegregationOtpRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SegregationOtpRequest));
            var result = await _companyParametersRepository.SegregationOtp(request.CompanyId, request.CurrentUserId, request.Method);
            var response = _mapper.Map<SegregationOtpResponse>(result);
            return response;
        }
    }
}
