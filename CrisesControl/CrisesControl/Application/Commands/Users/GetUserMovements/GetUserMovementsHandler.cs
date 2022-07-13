using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserMovements
{
    public class GetUserMovementsHandler : IRequestHandler<GetUserMovementsRequest, GetUserMovementsResponse>
    {
        private readonly GetUserMovementsValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserMovementsHandler(GetUserMovementsValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<GetUserMovementsResponse> Handle(GetUserMovementsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserMovementsRequest));

            //var userId = await _userRepository.GetUserMovements(request.ModuleId, request.UserId, request.XPos, request.YPos);
            return new GetUserMovementsResponse();
        }
    }
}
