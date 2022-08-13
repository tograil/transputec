using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.ApiUrlsById
{
    public class ApiUrlsByIdValidator:AbstractValidator<ApiUrlsByIdRequest>
    {
        public ApiUrlsByIdValidator()
        {
            RuleFor(x => x.ApiID).GreaterThan(0);
        }
    }
}
