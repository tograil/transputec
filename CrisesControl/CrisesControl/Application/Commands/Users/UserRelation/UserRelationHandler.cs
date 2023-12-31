﻿using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.UserRelation
{
    public class UserRelationHandler : IRequestHandler<UserRelationRequest, UserRelationResponse>
    {
        private readonly UserRelationValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserRelationHandler> _logger;

        public UserRelationHandler(UserRelationValidator userValidator, IUserRepository userService, ILogger<UserRelationHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<UserRelationResponse> Handle(UserRelationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UserRelationRequest));

            var userId = await _userRepository.UserRelations(cancellationToken);
            return new UserRelationResponse();
        }
    }
}
