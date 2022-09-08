using MediatR;

namespace CrisesControl.Api.Application.Commands.App.UpdatePushToken
{
    public class UpdatePushTokenHandler : IRequestHandler<UpdatePushTokenRequest, UpdatePushTokenResponse>
    {
        public Task<UpdatePushTokenResponse> Handle(UpdatePushTokenRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
