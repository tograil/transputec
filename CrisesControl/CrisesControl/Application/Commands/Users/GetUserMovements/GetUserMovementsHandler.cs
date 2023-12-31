﻿using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Infrastructure.Services;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserMovements
{
    public class GetUserMovementsHandler : IRequestHandler<GetUserMovementsRequest, GetUserMovementsResponse>
    {
        private readonly GetUserMovementsValidator _userValidator;
        private readonly IMessageService _MSG;
        private readonly ILogger<GetUserMovementsHandler> _logger;

        public GetUserMovementsHandler(GetUserMovementsValidator userValidator, IMessageService MSG, ILogger<GetUserMovementsHandler> logger)
        {
            _userValidator = userValidator;
            _MSG = MSG;
            _logger = logger;
        }

        public async Task<GetUserMovementsResponse> Handle(GetUserMovementsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserMovementsRequest));

            var userLocations = await _MSG.GetUserMovements(request.UserId);

            var response= new GetUserMovementsResponse();
            response.Locations = userLocations;
            return response;
        }
    }
}
