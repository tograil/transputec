using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.GetAllAddress
{
    public class GetAllAddressHandler : IRequestHandler<GetAllAddressRequest, GetAllAddressResponse>
    {
        public GetAllAddressHandler()
        {

        }
        public Task<GetAllAddressResponse> Handle(GetAllAddressRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
