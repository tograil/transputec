using MediatR;

namespace CrisesControl.Api.Application.Commands.Academy.SaveTourLog
{
    public class SaveTourLogRequest:IRequest<SaveTourLogResponse>
    {
        public string TourName { get; set; }
        public string TourKey { get; set; }
        public string StepKey { get; set; }
    }
}
