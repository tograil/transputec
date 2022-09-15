using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.SendFeedback
{
    public class SendFeedbackRequest:IRequest<SendFeedbackResponse>
    {
        public DeviceType DeviceType { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceModel { get; set; }
        public string FeedbackMessage { get; set; }
        public string UserEmail { get; set; }
    }
}
