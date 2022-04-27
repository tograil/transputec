using CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferenceList;

namespace CrisesControl.Api.Application.Query
{
    public interface ICommunicationQuery
    {
        public Task<GetUserActiveConferenceListResponse> GetUserActiveConferenceList(GetUserActiveConferenceListRequest request);
        //public Task<GetUserActiveConferenceListResponse> GetDepartment(GetUserActiveConferenceListRequest request);
    }
}
