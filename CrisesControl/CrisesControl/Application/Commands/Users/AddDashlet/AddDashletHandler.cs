using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;
using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.AddDashlet
{
    public class AddDashletHandler : IRequestHandler<AddDashletRequest, AddDashletResponse>
    {
        private readonly AddDashletValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AddDashletHandler(AddDashletValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<AddDashletResponse> Handle(AddDashletRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(AddDashletRequest));

            var userId = await _userRepository.AddDashlet(request.ModuleId, request.UserId, request.XPos, request.YPos);
            return new AddDashletResponse();
        }
    }
}
