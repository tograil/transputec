using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Incidents.Services;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Infrastructure.Services;

public class IncidentService : IIncidentService
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IActiveIncidentRepository _activeIncidentRepository;
    private readonly IActiveIncidentTaskService _activeIncidentTaskService;

    public IncidentService(IIncidentRepository incidentRepository,
        IMessageRepository messageRepository,
        IActiveIncidentRepository activeIncidentRepository, IActiveIncidentTaskService activeIncidentTaskService)
    {
        _incidentRepository = incidentRepository;
        _messageRepository = messageRepository;
        _activeIncidentRepository = activeIncidentRepository;
        _activeIncidentTaskService = activeIncidentTaskService;
    }

    public void InitiateIncident(IncidentActivation activation, IncidentSubset incidentSubset, CancellationToken cancellationToken = default)
    {
        _incidentRepository.AddIncidentActivation(activation, cancellationToken);

        if (incidentSubset.MessageMethod.Length > 0)
        {
            var pushAdded = false;
            var pushMethodId = 1;
            if (incidentSubset.TrackUser)
            {
                pushMethodId = _messageRepository.GetPushMethodId();
            }

            foreach (var method in incidentSubset.MessageMethod)
            {
                _messageRepository.CreateMessageMethod(0, method, activation.IncidentActivationId);
                if (pushMethodId == method)
                    pushAdded = true;
            }

            if (incidentSubset.TrackUser && !pushAdded)
            {
                 _messageRepository.CreateMessageMethod(0, pushMethodId,
                    activation.IncidentActivationId);
            }
        }

        if (incidentSubset.UsersToNotify.Length > 0)
        {
             _messageRepository.AddUserToNotify(0, incidentSubset.UsersToNotify,
                activation.IncidentActivationId);
        }

        if (incidentSubset.MultiResponse)
        {
             _messageRepository.SaveActiveMessageResponse(0, incidentSubset.AckOptions,
                activation.IncidentActivationId);
        }

        //TODO: Clarify nominated keyholders

         _activeIncidentRepository.ProcessImpactedLocation(incidentSubset.ImpactedLocationIds,
            activation.IncidentActivationId, activation.CompanyId, "INITIATE");

         _activeIncidentRepository.ProcessAffectedLocation(incidentSubset.AffectedLocations,
            activation.IncidentActivationId,
            activation.CompanyId);
    }

    public void InitiateAndLaunchIncident(IncidentActivation incidentActivation, IncidentSubset incidentSubset, CancellationToken cancellationToken = default)
    {
         _incidentRepository.AddIncidentActivation(incidentActivation, cancellationToken);

         _activeIncidentRepository.ProcessImpactedLocation(incidentSubset.ImpactedLocationId,
            incidentActivation.IncidentActivationId, incidentActivation.CompanyId, "COMBINED");

         _activeIncidentRepository.ProcessAffectedLocation(incidentSubset.AffectedLocations,
            incidentActivation.IncidentActivationId, incidentActivation.CompanyId, "COMBINED");

         _activeIncidentRepository.CreateActiveKeyContact(incidentActivation.IncidentActivationId,
            incidentActivation.IncidentId,
            incidentSubset.InitiateIncidentKeyHldLst, incidentSubset.UserId, incidentActivation.CompanyId, "GMT Standard Time");

        var priority = Common.GetPriority(incidentActivation.Severity);

        _messageRepository.DeleteMessageMethod(0, incidentActivation.IncidentActivationId);

        var messageId = _messageRepository.CreateMessage(incidentActivation.CompanyId,
            incidentActivation.IncidentDescription,
            "Incident", incidentActivation.IncidentActivationId, priority, incidentSubset.UserId, 1,
            DateTime.Now.GetDateTimeOffset(),
            incidentSubset.MultiResponse, incidentSubset.AckOptions, 99, incidentSubset.AudioAssetId, 0, incidentSubset.TrackUser,
            incidentSubset.SilentMessage,
            incidentSubset.MessageMethod).Result;

        if (incidentSubset.UsersToNotify != null)
        {
            _messageRepository.AddUserToNotify(messageId, incidentSubset.UsersToNotify,
                incidentActivation.IncidentActivationId);
        }

        _messageRepository.CreateIncidentNotificationList(incidentActivation.IncidentActivationId, messageId,
            incidentSubset.InitiateIncidentNotificationObjLst.ToList(), incidentSubset.UserId, incidentActivation.CompanyId);

        if (incidentSubset.HasTask)
        {
            _activeIncidentTaskService.StartTaskAllocation(incidentActivation.IncidentId,
                incidentActivation.IncidentActivationId, incidentSubset.UserId, incidentActivation.CompanyId);
        }

        _activeIncidentTaskService.CopyAssets(incidentActivation.IncidentId,
            incidentActivation.IncidentActivationId, messageId);
    }
}