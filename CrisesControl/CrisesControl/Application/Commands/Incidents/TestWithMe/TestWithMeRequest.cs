using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.TestWithMe
{
    public class TestWithMeRequest:IRequest<TestWithMeResponse>
    {
        public int IncidentId { get; set; }
        public int[] ImpactedLocationId { get; set; }
        
    }
}
