using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Addresses.UpdateAddress
{
    public class UpdateAddressValidator:AbstractValidator<UpdateAddressRequest>
    {
        public UpdateAddressValidator()
        {
            RuleFor(x => x.AddressId).GreaterThan(0);
        }
    }
}
