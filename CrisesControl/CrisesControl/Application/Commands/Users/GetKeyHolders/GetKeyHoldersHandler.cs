using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetKeyHolders
{
    public class GetKeyHoldersHandler : IRequestHandler<GetKeyHoldersRequest, GetKeyHoldersResponse>
    {
        private readonly GetKeyHoldersValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetKeyHoldersHandler(GetKeyHoldersValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<GetKeyHoldersResponse> Handle(GetKeyHoldersRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetKeyHoldersRequest));

            //var userId = await _userRepository.GetKeyHolders(request.ModuleId, request.UserId, request.XPos, request.YPos);
            return new GetKeyHoldersResponse();
        }
    }
}
