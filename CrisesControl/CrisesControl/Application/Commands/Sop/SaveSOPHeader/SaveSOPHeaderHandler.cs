using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.SaveSOPHeader
{
    public class SaveSOPHeaderHandler : IRequestHandler<SaveSOPHeaderRequest, SaveSOPHeaderResponse>
    {
        public Task<SaveSOPHeaderResponse> Handle(SaveSOPHeaderRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
