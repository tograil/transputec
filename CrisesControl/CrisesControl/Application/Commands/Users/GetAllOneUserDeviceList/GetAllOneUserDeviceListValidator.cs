using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetAllOneUserDeviceList
{
    public class GetAllOneUserDeviceListValidator:AbstractValidator<GetAllOneUserDeviceListRequest>
    {
        public GetAllOneUserDeviceListValidator()
        {
            RuleFor(x => x.QueriedUserId).GreaterThan(0);
        }
    }
}
