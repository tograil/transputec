using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageGroupList
{
    public class GetMessageGroupListValidator: AbstractValidator<GetMessageGroupListRequest>
    {
        public GetMessageGroupListValidator()
        {
            RuleFor(x => x.MessageID).GreaterThanOrEqualTo(0);
        }
    }
}
