using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.DeleteUserDevice
{
    public class DeleteUserDeviceValidator: AbstractValidator<DeleteUserDeviceRequest>
    {
        public DeleteUserDeviceValidator()
        {
            RuleFor(x => x.UserDeviceID)
                .GreaterThan(0);
        }
    }
}
