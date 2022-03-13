using CrisesControl.Api.Application.Commands.Common;
using CrisesControl.Core.Incidents;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddCompanyIncident;

public record AddCompanyIncidentRequest(int CompanyId, string IncidentIcon, string Name, string Description,
    int PlanAssetId, int IncidentTypeId, int Severity, int NumberOfKeyHolder, int CurrentUserId, string TimeZoneId,
    AddIncidentKeyHldLst[] AddIncidentKeyHldLst, int AudioAssetId, int Status = 1, bool TrackUser = false,
    bool SilentMessage = false, List<AckOption> AckOptions = null, bool IsSOS = false, int[] MessageMethod = null, int CascadePlanId = 0,
    int[] Groups = null, int[] KeyHolders = null) : IRequest<ResultResponse>;