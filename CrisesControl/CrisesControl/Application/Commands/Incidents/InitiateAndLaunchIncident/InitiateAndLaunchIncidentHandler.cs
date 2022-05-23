using System.Data.SqlTypes;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Incidents.Services;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.SharedKernel.Utils;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.InitiateAndLaunchIncident;

public class InitiateAndLaunchIncidentHandler 
    : IRequestHandler<InitiateAndLaunchIncidentRequest, InitiateAndLaunchIncidentResponse>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly ICurrentUser _currentUser;
    private readonly ICompanyRepository _companyRepository;
    private readonly IActiveIncidentRepository _activeIncidentRepository;
    private readonly IIncidentService _incidentService;

    public InitiateAndLaunchIncidentHandler(IIncidentRepository incidentRepository,
        ICurrentUser currentUser,
        ICompanyRepository companyRepository,
        IActiveIncidentRepository activeIncidentRepository,
        IIncidentService incidentService)
    {
        _incidentRepository = incidentRepository;
        _currentUser = currentUser;
        _companyRepository = companyRepository;
        _activeIncidentRepository = activeIncidentRepository;
        _incidentService = incidentService;
    }

    public async Task<InitiateAndLaunchIncidentResponse> Handle(InitiateAndLaunchIncidentRequest request, CancellationToken cancellationToken)
    {
        var incidentToVerify = await _incidentRepository.GetIncident(_currentUser.CompanyId, request.IncidentId);

        if (incidentToVerify is not null)
        {
            var simulationText = string.Empty;
            if (request.LaunchMode == 4)
            {
                simulationText =
                    await _companyRepository.GetCompanyParameter("INCIDENT_SIMULATION_TEXT", _currentUser.CompanyId) +
                    " ";
            }

            var incidentActivation = new IncidentActivation
            {
                Name = (simulationText + incidentToVerify.Name).Trim(),
                IncidentIcon = incidentToVerify.IncidentIcon,
                CompanyId = _currentUser.CompanyId,
                IncidentId = request.IncidentId,
                IncidentDescription = request.Description.Trim(),
                Severity = request.Severity,
                ImpactedLocationId = request.ImpactedLocationId.FirstOrDefault(),
                InitiatedOn = DateTime.Now.GetDateTimeOffset(),
                InitiatedBy = _currentUser.UserId,
                LaunchedOn = DateTime.Now.GetDateTimeOffset(),
                LaunchedBy = _currentUser.UserId,
                Status = 2,
                TrackUser = request.TrackUser,
                SilentMessage = request.SilentMessage,
                CreatedBy = _currentUser.UserId,
                CreatedOn = DateTime.Now.GetDateTimeOffset(),
                DeactivatedOn = (DateTime)SqlDateTime.Null,
                ClosedOn = (DateTime)SqlDateTime.Null,
                UpdatedBy = _currentUser.UserId,
                UpdatedOn = DateTime.Now.GetDateTimeOffset(),
                AssetId = request.AudioAssetId,
                HasTask = incidentToVerify.HasTask,
                LaunchMode = request.LaunchMode,
                SocialHandle = string.Join(",", request.SocialHandle),
                CascadePlanId = request.CascadePlanId
            };

            var incidentSubset = new IncidentSubset
            {
                UserId = _currentUser.UserId,
                AckOptions = request.AckOptions,
                MessageMethod = request.MessageMethod,
                MultiResponse = request.MultiResponse,
                TrackUser = request.TrackUser,
                UsersToNotify = request.UsersToNotify,
                ImpactedLocationIds = request.ImpactedLocationId,
                AffectedLocations = request.AffectedLocations,
                InitiateIncidentKeyHldLst = request.InitiateIncidentKeyHldLst,
                AudioAssetId = request.AudioAssetId,
                SilentMessage = request.SilentMessage,
                InitiateIncidentNotificationObjLst = request.InitiateIncidentNotificationObjLst,
                HasTask = incidentToVerify.HasTask
            };

            await _incidentService.InitiateAndLaunchIncident(incidentActivation, incidentSubset, cancellationToken);

            var incidents =
                await _activeIncidentRepository.GetIncidentActivationList(incidentActivation.IncidentActivationId,
                    _currentUser.CompanyId);

            return new InitiateAndLaunchIncidentResponse
            {
                IncidentActivationId = incidents.FirstOrDefault()?.IncidentActivationId ?? 0,
                ErrorCode = "0"
            };
        }

        return null!;
    }
}