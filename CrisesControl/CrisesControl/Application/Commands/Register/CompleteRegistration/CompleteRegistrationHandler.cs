using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Register.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.CompleteRegistration
{
    public class CompleteRegistrationHandler:IRequestHandler<CompleteRegistrationRequest,CompleteRegistrationResponse>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IMapper _mapper;
        public CompleteRegistrationHandler(IRegisterRepository registerRepository, IMapper mapper)
        {
            _registerRepository = registerRepository;
            _mapper = mapper;
        }

        public async Task<CompleteRegistrationResponse> Handle(CompleteRegistrationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CompleteRegistrationRequest));
            var mappedRequest = _mapper.Map<Core.Register.TempRegister>(request);
            var result = await _registerRepository.CompleteRegistration(mappedRequest);
            var response = _mapper.Map<CompleteRegistrationResponse>(result);
            return response;
        }
    }
}
