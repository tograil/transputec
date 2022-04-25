using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferenceList
{
    public class GetUserActiveConferenceListRequest : IRequest<GetUserActiveConferenceListResponse>
    {
        public int UserID { get; set; }
        public int CompanyID { get; set; }
    }
}
