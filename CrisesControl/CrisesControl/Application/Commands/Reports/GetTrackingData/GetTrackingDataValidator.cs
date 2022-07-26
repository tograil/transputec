using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.GetTrackingData
{
    public class GetTrackingDataValidator:AbstractValidator<GetTrackingDataRequest>
    {
        public GetTrackingDataValidator()
        {
            RuleFor(x => x.TrackMeID).GreaterThan(0);
            RuleFor(x => x.UserDeviceID).GreaterThan(0);
            RuleFor(x => x.EndDate).GreaterThan(x=>x.StartDate);
            RuleFor(x => x.StartDate).LessThanOrEqualTo(DateTimeOffset.Now);
        }
    }
}
