using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetAllUsersDevice
{
    public class GetAllUserDeviceHandler : IRequestHandler<GetAllUserDevicesRequest, GetAllUserDevicesResponse>
    {
        private readonly GetAllUserDeviceValidator _userValidator;
        private readonly IUserQuery _userQuery;
        private readonly ILogger<GetAllUserDeviceHandler> _logger;

        public GetAllUserDeviceHandler(GetAllUserDeviceValidator userValidator, IUserQuery userService, ILogger<GetAllUserDeviceHandler> logger)
        {
            _userValidator = userValidator;
            _userQuery = userService;
            _logger = logger;
        }
        public async Task<GetAllUserDevicesResponse> Handle(GetAllUserDevicesRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAllUserDevicesRequest));
            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _userQuery.GetAllUserDeviceList(request, cancellationToken);
            return response;
        }
    }
}
