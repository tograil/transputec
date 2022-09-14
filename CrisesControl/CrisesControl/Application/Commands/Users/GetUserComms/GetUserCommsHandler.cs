using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserComms
{
    public class GetUserCommsHandler : IRequestHandler<GetUserCommsRequest, GetUserCommsResponse>
    {
        private readonly GetUserCommsValidator _userValidator;
        private readonly IUserQuery _userQuery;
        private readonly ILogger<GetUserCommsHandler> _logger;

        public GetUserCommsHandler(GetUserCommsValidator userValidator, IUserQuery userService, ILogger<GetUserCommsHandler> logger)
        {
            _userValidator = userValidator;
            _userQuery = userService;
            _logger = logger;
        }
        public async Task<GetUserCommsResponse> Handle(GetUserCommsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserCommsRequest));
            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _userQuery.GetUserComms(request, cancellationToken);
            return (GetUserCommsResponse)response;
        }
    }
}
