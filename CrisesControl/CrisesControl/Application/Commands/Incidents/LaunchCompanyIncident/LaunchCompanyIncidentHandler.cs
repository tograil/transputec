using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.SharedKernel.Utils;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.LaunchCompanyIncident;

public class LaunchCompanyIncidentHandler : IRequestHandler<LaunchCompanyIncidentRequest, LaunchCompanyIncidentResponse>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IActiveIncidentRepository _activeIncidentRepository;

    public LaunchCompanyIncidentHandler(IIncidentRepository incidentRepository,
        ICurrentUser currentUser,
        IActiveIncidentRepository activeIncidentRepository)
    {
        _incidentRepository = incidentRepository;
        _currentUser = currentUser;
        _activeIncidentRepository = activeIncidentRepository;
    }

    public async Task<LaunchCompanyIncidentResponse> Handle(LaunchCompanyIncidentRequest request, CancellationToken cancellationToken)
    {
        var incidentActivation =
            await _incidentRepository.GetIncidentActivation(_currentUser.CompanyId, request.IncidentActivationtId);

        if (incidentActivation == null) return null!;

        incidentActivation.IncidentIcon = incidentActivation.Incident.IncidentIcon;
        incidentActivation.IncidentDescription = request.Description;
        incidentActivation.Severity = request.Severity;
        incidentActivation.ImpactedLocationId = request.ImpactedLocationId.FirstOrDefault();
        incidentActivation.Status = 2;
        incidentActivation.LaunchedOn = DateTime.Now.GetDateTimeOffset();
        incidentActivation.LaunchedBy = _currentUser.UserId;
        incidentActivation.UpdatedOn = DateTime.Now.GetDateTimeOffset();
        incidentActivation.UpdatedBy = _currentUser.UserId;
        incidentActivation.TrackUser = request.TrackUser;
        incidentActivation.SilentMessage = request.SilentMessage;

        if (incidentActivation.AssetId != request.AudioAssetId)
            incidentActivation.AssetId = request.AudioAssetId;

        incidentActivation.HasTask = incidentActivation.Incident.HasTask;

        await _incidentRepository.UpdateIncidentActivation(incidentActivation, cancellationToken);

        await _activeIncidentRepository.ProcessImpactedLocation(request.ImpactedLocationId,
            request.IncidentActivationtId, _currentUser.CompanyId, "LAUNCH");

        await _activeIncidentRepository.ProcessAffectedLocation(request.AffectedLocations,
            request.IncidentActivationtId, _currentUser.CompanyId, "LAUNCH");

        await _activeIncidentRepository.CreateActiveKeyContact(request.IncidentActivationtId,
            incidentActivation.IncidentId,
            request.LaunchIncidentKeyHldLst, _currentUser.UserId, _currentUser.CompanyId, "GMT Standard Time");

        return new LaunchCompanyIncidentResponse
        {
            IncidentActivationId = incidentActivation.IncidentActivationId,
            ErrorCode = "0"
        };
    }
}