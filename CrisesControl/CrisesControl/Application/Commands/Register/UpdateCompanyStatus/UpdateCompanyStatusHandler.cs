using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Register;
using CrisesControl.Core.Register.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.UpdateCompanyStatus
{
    public class UpdateCompanyStatusHandler : IRequestHandler<UpdateCompanyStatusRequest, UpdateCompanyStatusResponse>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IMapper _mapper;
        public UpdateCompanyStatusHandler(IRegisterRepository registerRepository, IMapper mapper)
        {
            _registerRepository = registerRepository;
            _mapper = mapper;
        }

        public async Task<UpdateCompanyStatusResponse> Handle(UpdateCompanyStatusRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateCompanyStatusRequest));
            var response = new UpdateCompanyStatusResponse();
            var mappedRequest = _mapper.Map<ViewCompanyModel>(request);
            response.Result = await _registerRepository.UpdateCompanyStatus(mappedRequest);
            return response;
        }
    }
}
