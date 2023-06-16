using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.CopyIncident;

public class CopyIncidentHandler : IRequestHandler<CopyIncidentRequest, bool>
{
    private readonly ICurrentUser _currentUser;
    private readonly IIncidentRepository _incidentRepository;

    public CopyIncidentHandler(ICurrentUser currentUser, IIncidentRepository incidentRepository)
    {
        _currentUser = currentUser;
        _incidentRepository = incidentRepository;
    }

    public async Task<bool> Handle(CopyIncidentRequest request, CancellationToken cancellationToken)
    {
        await _incidentRepository.CopyIncidentToCompany(_currentUser.CompanyId, _currentUser.UserId);

        return true;
    }
}