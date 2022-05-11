﻿using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck
{
    public class GetIndidentMessageNoAckRequest: IRequest<GetIndidentMessageNoAckResponse>
    {
        public int IncidentActivationId { get; set; }
        public int RecordStart { get; set; }
        public int RecordLength { get; set; }
        public string SearchString { get; set; }     
        public string OrderBy { get; set; } 
        public string OrderDir { get; set; }
        public int draw { get; set; }

    }
}
