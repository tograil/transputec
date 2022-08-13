using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.DeleteApiUrl
{
    public class DeleteApiUrlRequest:IRequest<DeleteApiUrlResponse>
    {
        public int ApiID { get; set; }
    }
}
