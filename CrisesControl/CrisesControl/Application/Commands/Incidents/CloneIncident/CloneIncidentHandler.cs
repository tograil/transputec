using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.CloneIncident;

public class CloneIncidentHandler : IRequestHandler<CloneIncidentRequest, NewIncident>
{
    private readonly ICurrentUser _currentUser;
    private readonly IIncidentRepository _incidentRepository;

    public CloneIncidentHandler(ICurrentUser currentUser, IIncidentRepository incidentRepository)
    {
        _currentUser = currentUser;
        _incidentRepository = incidentRepository;
    }

    public Task<NewIncident> Handle(CloneIncidentRequest request, CancellationToken cancellationToken)
    {
        var newIncident = _incidentRepository.CloneIncident(request.IncidentId, request.KeepKeyContact,
            request.KeepIncidentMessage, request.KeepTasks
            , request.KeepIncidentAsset, request.KeepTaskAssets, request.KeepTaskCheckList,
            request.KeepIncidentParticipants, request.Status, _currentUser.UserId, _currentUser.CompanyId,
            "GMT Standard Time");

        return Task.FromResult(newIncident);
    }
}