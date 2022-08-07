using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.ApiUrlsById
{
    public class ApiUrlsByIdRequest:IRequest<ApiUrlsByIdResponse>
    {
        public int ApiID { get; set; }
    }
}
