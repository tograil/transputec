﻿using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GetAllGroup
{
    public class GetAllGroupRequest : IRequest<GetAllGroupResponse>
    {
        public int CompanyId { get; set; }
        public int IncidentId { get; set; }
        public bool FilterVirtual { get; set; }

    }
}
