using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.TrackUserDevice
{
    public class TrackUserDeviceHandler : IRequestHandler<TrackUserDeviceRequest, TrackUserDeviceResponse>
    {
        private readonly TrackUserDeviceValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TrackUserDeviceHandler(TrackUserDeviceValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<TrackUserDeviceResponse> Handle(TrackUserDeviceRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TrackUserDeviceRequest));

            //var userId = await _userRepository.TrackUserDevice(request.ModuleItems, request.ModulePage, request.UserID, cancellationToken);
            return new TrackUserDeviceResponse();
        }
    }
}
