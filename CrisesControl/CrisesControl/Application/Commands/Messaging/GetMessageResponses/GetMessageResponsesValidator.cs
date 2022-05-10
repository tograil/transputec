using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageResponses {

    public class GetMessageResponsesValidator : AbstractValidator<GetMessageResponsesRequest> {
        public GetMessageResponsesValidator() {
            RuleFor(x => x.MessageType).Must(list => new string[] { "ALL", "Ping", "Incident", "Checklist" }.Contains(list));
            RuleFor(x=>x.Status).GreaterThanOrEqualTo(-1).LessThan(4).ToString();
        }
    }
}
