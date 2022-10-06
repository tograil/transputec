using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.TrackUserDevice
{
    public class TrackUserDeviceHandler : IRequestHandler<TrackUserDeviceRequest, TrackUserDeviceResponse>
    {
        private readonly TrackUserDeviceValidator _userValidator;
        private readonly IDBCommonRepository _DBC;
        private readonly ILogger<TrackUserDeviceHandler> _logger;

        public TrackUserDeviceHandler(TrackUserDeviceValidator userValidator, IUserRepository userService, ILogger<TrackUserDeviceHandler> logger, IDBCommonRepository DBC)
        {
            _userValidator = userValidator;
           // _userRepository = userService;
            _logger = logger;
            _DBC = DBC;
        }

        public async Task<TrackUserDeviceResponse> Handle(TrackUserDeviceRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TrackUserDeviceRequest));
            await _userValidator.ValidateAndThrowAsync(request,cancellationToken);
             var trackDevice=await _DBC.AddUserTrackingDevices(request.UserId);
            if (trackDevice)
            {
                return new TrackUserDeviceResponse()
                {
                    TrackedDevice = trackDevice,
                    Message = "User has Device"

                };

            }
            return new TrackUserDeviceResponse()
            {
                TrackedDevice = false,
                Message = "No device found"

            };
        }
    }
}
