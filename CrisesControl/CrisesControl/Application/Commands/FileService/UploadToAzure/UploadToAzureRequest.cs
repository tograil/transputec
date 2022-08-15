using MediatR;

namespace CrisesControl.Api.Application.Commands.FileService.UploadToAzure
{
    public class UploadToAzureRequest:IRequest<HttpResponseMessage>
    {
    }
}
