using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdateApiUrls
{
    public class UpdateApiUrlsRequest:IRequest<UpdateApiUrlsResponse>
    {
        public CrisesControl.Core.Administrator.Api Api { get; set; }
    }
}
