using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.TrackUserDevice
{
    public class TrackUserDeviceValidator : AbstractValidator<TrackUserDeviceRequest>
    {
        public TrackUserDeviceValidator()
        {
        }
    }
}
