using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Addresses.DeleteAddress
{
    public class DeleteAddressValidator:AbstractValidator<DeleteAddressRequest>
    {
        public DeleteAddressValidator()
        {
            RuleFor(x=>x.AddressId).GreaterThan(0);
        }
    }
}
