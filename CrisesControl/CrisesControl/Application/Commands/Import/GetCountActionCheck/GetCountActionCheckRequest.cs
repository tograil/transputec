using MediatR;

namespace CrisesControl.Api.Application.Commands.Import.GetCountActionCheck
{
    public class GetCountActionCheckRequest : IRequest<GetCountActionCheckResponse>
    {
        public string SessionId { get; set; }
        public int OutUserCompanyId { get; set; }
    }
}
