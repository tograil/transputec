using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.OffDutySetting
{
    public class OffDutySettingRequest : IRequest<OffDutySettingResponse>
    {
        public string OffDutyAction { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public bool AllowPush { get; set; }
        public bool AllowPhone { get; set; }
        public bool AllowText { get; set; }
        public bool AllowEmail { get; set; }
        public string Source { get; set; }
    }
}
