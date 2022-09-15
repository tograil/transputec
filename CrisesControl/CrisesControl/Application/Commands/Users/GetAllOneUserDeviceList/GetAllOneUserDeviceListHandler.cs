using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetAllOneUserDeviceList
{
    public class GetAllOneUserDeviceListHandler : IRequestHandler<GetAllOneUserDeviceListRequest, GetAllOneUserDeviceListResponse>
    {
        private readonly GetAllOneUserDeviceListValidator _userValidator;
        private readonly IUserQuery _userQuery;
        private readonly ILogger<GetAllOneUserDeviceListHandler> _logger;

        public GetAllOneUserDeviceListHandler(GetAllOneUserDeviceListValidator userValidator, IUserQuery userService, ILogger<GetAllOneUserDeviceListHandler> logger)
        {
            _userValidator = userValidator;
            _userQuery = userService;
            _logger = logger;
        }
        public async Task<GetAllOneUserDeviceListResponse> Handle(GetAllOneUserDeviceListRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAllOneUserDeviceListRequest));
            await _userValidator.ValidateAndThrowAsync(request,cancellationToken);
            var response= await _userQuery.GetAllOneUserDeviceList(request,cancellationToken);
            return (GetAllOneUserDeviceListResponse)response;
        }
    }
}
