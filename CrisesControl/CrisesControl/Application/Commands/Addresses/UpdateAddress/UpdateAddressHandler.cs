using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.UpdateAddress
{
    public class UpdateAddressHandler:IRequestHandler<UpdateAddressRequest, UpdateAddressResponse>
    {
        public UpdateAddressHandler()
        {

        }

        public Task<UpdateAddressResponse> Handle(UpdateAddressRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
