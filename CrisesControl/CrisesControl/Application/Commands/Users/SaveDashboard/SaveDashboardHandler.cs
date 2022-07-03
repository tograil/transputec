using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.SaveDashboard
{
    public class SaveDashboardHandler : IRequestHandler<SaveDashboardRequest, SaveDashboardResponse>
    {
        private readonly SaveDashboardValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public SaveDashboardHandler(SaveDashboardValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<SaveDashboardResponse> Handle(SaveDashboardRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveDashboardRequest));

            var userId = await _userRepository.SaveDashboard(request.ModuleItems, request.ModulePage, request.UserID, cancellationToken);
            return new SaveDashboardResponse();
        }
    }
}
