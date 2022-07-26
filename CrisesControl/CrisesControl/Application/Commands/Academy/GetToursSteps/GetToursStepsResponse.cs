using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Academy.GetToursSteps
{
    public class GetToursStepsResponse
    {
        public List<TourStep> Data { get; set; }
        public string Message { get; set; }
    }
}
