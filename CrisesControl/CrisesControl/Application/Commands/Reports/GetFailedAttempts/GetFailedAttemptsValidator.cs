using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetFailedAttempts
{
    public class GetFailedAttemptsValidator:AbstractValidator<GetFailedAttemptsRequest>
    {
        public GetFailedAttemptsValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.CommsMethod));
            RuleFor(x => x.MessageListID).GreaterThan(0);
        }
    }
}
