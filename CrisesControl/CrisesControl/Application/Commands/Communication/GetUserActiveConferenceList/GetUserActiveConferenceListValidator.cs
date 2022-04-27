using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferenceList
{
    public class GetUserActiveConferenceListValidator: AbstractValidator<GetUserActiveConferenceListRequest>
    {
        public GetUserActiveConferenceListValidator()
        {
            RuleFor( x => x.CompanyID)
                .GreaterThan(0);
            RuleFor(x => x.UserID)
                .GreaterThan(0);

        }
    }
}
