﻿using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncident
{
    public class UpdateLibIncidentRequest:IRequest<UpdateLibIncidentResponse>
    {
        public int LibIncidentId { get; set; }
       
        public string Name { get; set; }
        public string Description { get; set; }
        public int LibIncidentTypeId { get; set; }
      
        public string LibIncidentIcon { get; set; }
        public int Severity { get; set; }
        public int Status { get; set; }
    }
}
