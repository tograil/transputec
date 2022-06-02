using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Messaging.GetReplies
{
    public class GetRepliesValidator : AbstractValidator<GetRepliesRequest>
    {
        public GetRepliesValidator()
        {
            RuleFor(x => x.MessageId).GreaterThan(0).ToString();
        }
    }
}
