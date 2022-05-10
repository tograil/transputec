using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessages {
    public class GetMessagesValidator : AbstractValidator<GetMessagesRequest> {
        public GetMessagesValidator() {
            RuleFor(x => x.MessageType).Must(list => new string[] { "Ping", "Incident" }.Contains(list));
            RuleFor(x => x.TargetUserId).GreaterThanOrEqualTo(0);
            RuleFor(x => x.IncidentActivationId).GreaterThanOrEqualTo(0);
        }
    }
}
