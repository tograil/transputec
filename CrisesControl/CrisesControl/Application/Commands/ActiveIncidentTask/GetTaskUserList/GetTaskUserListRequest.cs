using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskUserList
{
    public class GetTaskUserListRequest:IRequest<GetTaskUserListResponse>
    {
        public int OutLoginUserId { get; set; }
        public int OutLoginCompanyId { get; set; }
        public Search Search { get; set; }
        public string TypeName { get; set; }
        public int ActiveIncidentTaskID { get; set; }
        public  string CompanyKey { get; set; }
        public int draw { get; set; }
    }
}
