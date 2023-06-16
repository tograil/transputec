using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.DeleteAddress
{
    public class DeleteAddressRequest:IRequest<DeleteAddressResponse>
    {
        public int AddressId { get; set; }
    }
}
