using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserCount
{
    public class GetUserCountHandler : IRequestHandler<GetUserCountRequest, GetUserCountResponse>
    {
        private readonly GetUserCountValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserCountHandler(GetUserCountValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<GetUserCountResponse> Handle(GetUserCountRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserCountRequest));

            //var userId = await _userRepository.GetUserCount(request.ModuleId, request.UserId, request.XPos, request.YPos);
            return new GetUserCountResponse();
        }
    }
}
