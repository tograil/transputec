using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTimezone
{
    public class GetTimezoneRequest : IRequest<GetTimezoneResponse>
    {
    }
}
