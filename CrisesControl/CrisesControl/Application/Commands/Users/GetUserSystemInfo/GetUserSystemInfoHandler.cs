﻿using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserSystemInfo
{
    public class GetUserSystemInfoHandler : IRequestHandler<GetUserSystemInfoRequest, GetUserSystemInfoResponse>
    {
        private readonly GetUserSystemInfoValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserSystemInfoHandler> _logger;

        public GetUserSystemInfoHandler(GetUserSystemInfoValidator userValidator, IUserRepository userService, ILogger<GetUserSystemInfoHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<GetUserSystemInfoResponse> Handle(GetUserSystemInfoRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserSystemInfoRequest));

            var userId = await _userRepository.GetUserSystemInfo(request.QUserId, request.CompanyId);
            return new GetUserSystemInfoResponse();
        }
    }
}
