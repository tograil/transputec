using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.GetAddress
{
    public class GetAddressHandler:IRequestHandler<GetAddressRequest, GetAddressResponse>
    {
        public GetAddressHandler()
        {

        }

        public Task<GetAddressResponse> Handle(GetAddressRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
