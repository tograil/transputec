using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetResponseCoordinates
{
    public class GetResponseCoordinatesValidator:AbstractValidator<GetResponseCoordinatesRequest>
    {
        public GetResponseCoordinatesValidator()
        {
            RuleFor(x => x.MessageId).GreaterThan(0);
        }
    }
}
