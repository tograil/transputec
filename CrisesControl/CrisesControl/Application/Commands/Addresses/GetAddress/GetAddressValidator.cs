using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Addresses.GetAddress
{
    public class GetAddressValidator:AbstractValidator<GetAddressRequest>
    {
        public GetAddressValidator()
        {
            RuleFor(x => x.AddressId).GreaterThan(0);
        }
    }
}
