﻿using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferences {
    public class GetUserActiveConferencesRequest : IRequest<GetUserActiveConferencesResponse> {
        public int CompanyId { get; set; }
    }
}
