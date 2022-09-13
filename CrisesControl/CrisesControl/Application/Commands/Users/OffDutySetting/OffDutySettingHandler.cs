using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.OffDutySetting
{
    public class OffDutySettingHandler : IRequestHandler<OffDutySettingRequest, OffDutySettingResponse>
    {
        private readonly OffDutySettingValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OffDutySettingHandler> _logger;

        public OffDutySettingHandler(OffDutySettingValidator userValidator, IUserRepository userService, ILogger<OffDutySettingHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<OffDutySettingResponse> Handle(OffDutySettingRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(OffDutySettingRequest));

            //var userId = await _userRepository.OffDutySetting(request.ModuleId, request.UserId, request.XPos, request.YPos);
            return new OffDutySettingResponse();
        }
    }
}
