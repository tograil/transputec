using CrisesControl.Api.Application.Commands.Common;
using CrisesControl.Core.Paging;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAllActiveCompanyIncident;

public class GetAllActiveCompanyIncidentRequest : PagedRequest, IRequest<GetAllActiveCompanyIncidentResponse>
{
    public string? Status { get; set; }
    // properties are not capital due to json mapping
    //public int draw { get; set; }
    //public int start { get; set; }
    //public int length { get; set; }
    //public Search? search { get; set; }
    //public List<Order>? order { get; set; }

    //public int ActiveIncidentID { get; set; }
    //public int ActiveIncidentTaskID { get; set; }
    //public string TaskActionReason { get; set; }
    //public string TaskCompletionNote { get; set; }
    //public List<int> ParticipantGroupID { get; set; }
    //public List<int> ParticipantUserID { get; set; }
    //public int ReallocateTo { get; set; }
    //public int ReassignTo { get; set; }
    //public string TypeName { get; set; }
    //public string[] SendUpdateTo { get; set; }
    //public List<NotificationUserList> MembersToNotify { get; set; }
    //public int[] MessageMethod { get; set; }
    //public string Status { get; set; }
    //public string Note { get; set; }
    //public int CascadePlanID { get; set; }
    //public int[] TaskAssets { get; set; }
    //public int[] DelegateTo { get; set; }

    ////Pagination
    //public bool SkipDeleted { get; set; }
    //public bool ActiveOnly { get; set; }
    //public bool SkipInActive { get; set; }
    //public bool KeyHolderOnly { get; set; }
    //// properties are not capital due to json mapping
    //public int draw { get; set; }
    //public int start { get; set; }
    //public int length { get; set; }
    //public List<Column> columns { get; set; }
    //public Search search { get; set; }
    //public List<Order> order { get; set; }
    //public string UniqueKey { get; set; }
    //public string Filters { get; set; }
}