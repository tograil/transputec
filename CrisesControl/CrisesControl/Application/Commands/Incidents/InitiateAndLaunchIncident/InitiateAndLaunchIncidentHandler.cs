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
    private readonly IMessageRepository _messageRepository;
    private readonly IActiveIncidentTaskService _activeIncidentTaskService;

    public InitiateAndLaunchIncidentHandler(IIncidentRepository incidentRepository,
        ICurrentUser currentUser,
        ICompanyRepository companyRepository,
        IActiveIncidentRepository activeIncidentRepository,
        IMessageRepository messageRepository,
        IActiveIncidentTaskService activeIncidentTaskService)
    {
        _incidentRepository = incidentRepository;
        _currentUser = currentUser;
        _companyRepository = companyRepository;
        _activeIncidentRepository = activeIncidentRepository;
        _messageRepository = messageRepository;
        _activeIncidentTaskService = activeIncidentTaskService;
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

            await _incidentRepository.AddIncidentActivation(incidentActivation, cancellationToken);

            await _activeIncidentRepository.ProcessImpactedLocation(request.ImpactedLocationId,
                incidentActivation.IncidentActivationId, _currentUser.CompanyId, "COMBINED");

            await _activeIncidentRepository.ProcessAffectedLocation(request.AffectedLocations,
                incidentActivation.IncidentActivationId, _currentUser.CompanyId, "COMBINED");

            await _activeIncidentRepository.CreateActiveKeyContact(incidentActivation.IncidentActivationId,
                incidentActivation.IncidentId,
                request.InitiateIncidentKeyHldLst, _currentUser.UserId, _currentUser.CompanyId, "GMT Standard Time");

            var priority = SharedKernel.Utils.Common.GetPriority(incidentActivation.Severity);

            await _messageRepository.DeleteMessageMethod(0, incidentActivation.IncidentActivationId);

            var messageId = await _messageRepository.CreateMessage(_currentUser.CompanyId,
                incidentActivation.IncidentDescription,
                "Incident", incidentActivation.IncidentActivationId, priority, _currentUser.UserId, 1,
                DateTime.Now.GetDateTimeOffset(),
                request.MultiResponse, request.AckOptions, 99, request.AudioAssetId, 0, request.TrackUser,
                request.SilentMessage,
                request.MessageMethod);

            if (request.UsersToNotify != null)
            {
                await _messageRepository.AddUserToNotify(messageId, request.UsersToNotify,
                    incidentActivation.IncidentActivationId);
            }

            await _messageRepository.CreateIncidentNotificationList(incidentActivation.IncidentActivationId, messageId,
                request.InitiateIncidentNotificationObjLst.ToList(), _currentUser.UserId, _currentUser.CompanyId);

            if (incidentToVerify.HasTask)
            {
                await _activeIncidentTaskService.StartTaskAllocation(incidentActivation.IncidentId,
                    incidentActivation.IncidentActivationId, _currentUser.UserId, _currentUser.CompanyId);
            }

            await _activeIncidentTaskService.CopyAssets(incidentActivation.IncidentId,
                incidentActivation.IncidentActivationId, messageId);

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