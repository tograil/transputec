using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageAnslysisResponse
{
    public class GetMessageAnslysisResponseValidator:AbstractValidator<GetMessageAnslysisResponseRequest>
    {
        public GetMessageAnslysisResponseValidator()
        {
            RuleFor(x => x.MessageId).GreaterThan(0);
        }
    }
}
