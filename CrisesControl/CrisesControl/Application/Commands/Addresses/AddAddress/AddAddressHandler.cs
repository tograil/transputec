using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.AddAddress
{
    public class AddAddressHandler : IRequestHandler<AddAddressRequest, AddAddressResponse>
    {
        public AddAddressHandler()
        {

        }
        public Task<AddAddressResponse> Handle(AddAddressRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
