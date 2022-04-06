using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUser
{
    public class GetUserHandler: IRequestHandler<GetUserRequest, GetUserResponse>
    {
        private readonly GetUserValidator _userValidator;
        private readonly IUserQuery _userQuery;

        public GetUserHandler(GetUserValidator userValidator, IUserQuery userQuery)
        {
            _userValidator = userValidator;
            _userQuery = userQuery;
        }

        public async Task<GetUserResponse> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserRequest));
            
            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var user = await _userQuery.GetUser(request);

            return user;
        }
    }
}
