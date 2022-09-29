using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetSOSItems {
    public class GetSOSItemsRequest : IRequest<GetSOSItemsResponse>{
        public int UserId { get; set; }
    }
}
