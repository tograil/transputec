using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.GetAddress
{
    public class GetAddressRequest:IRequest<GetAddressResponse>
    {
        public int AddressId { get; set; }
    }
}
