﻿using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.GetLocation
{
    public class GetLocationHandler: IRequestHandler<GetLocationRequest, GetLocationResponse>
    {
        private readonly GetLocationValidator _locationValidator;
        private readonly ILocationQuery _locationQuery;

        public GetLocationHandler(GetLocationValidator locationValidator, ILocationQuery locationQuery)
        {
            _locationValidator = locationValidator;
            _locationQuery = locationQuery;
        }

        public async Task<GetLocationResponse> Handle(GetLocationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetLocationRequest));
            
            await _locationValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var departments = await _locationQuery.GetLocation(request);

            return new GetLocationResponse();
        }
    }
}
