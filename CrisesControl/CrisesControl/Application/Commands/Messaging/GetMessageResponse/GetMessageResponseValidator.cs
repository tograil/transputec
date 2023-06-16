using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageResponse {
    public class GetMessageResponseValidator : AbstractValidator<GetMessageResponseRequest> {
        public GetMessageResponseValidator() {
            RuleFor(x => x.MessageType).Must(list => new string[] { "ALL", "Ping", "Incident", "Checklist" }.Contains(list));
            RuleFor(x => x.ResponseID).GreaterThan(0);
        }
    }
}
