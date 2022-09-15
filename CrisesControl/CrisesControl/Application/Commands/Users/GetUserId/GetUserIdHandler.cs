using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserId
{
    public class GetUserIdHandler : IRequestHandler<GetUserIdRequest, GetUserIdResponse>
    {
        private readonly GetUserIdValidator _userValidator;
        private readonly IUserQuery _userQuery;
        private readonly ILogger<GetUserIdHandler> _logger;

        public GetUserIdHandler(GetUserIdValidator userValidator, IUserQuery userService, ILogger<GetUserIdHandler> logger)
        {
            _userValidator = userValidator;
            _userQuery = userService;
            _logger = logger;
        }
        public async Task<GetUserIdResponse> Handle(GetUserIdRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserIdRequest));
            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await _userQuery.GetUserId(request);
            return response;
        }
    }
}
