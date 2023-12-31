﻿using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentLibrary
{
    public class GetIncidentLibraryHandler : IRequestHandler<GetIncidentLibraryRequest, GetIncidentLibraryResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentLibraryHandler> _logger;
        public GetIncidentLibraryHandler(ILogger<GetIncidentLibraryHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetIncidentLibraryResponse> Handle(GetIncidentLibraryRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetIncidentLibrary(request);
            return result;
        }
    }
}
