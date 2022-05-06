using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Incidents.Services;
using CrisesControl.Core.Messages.Repositories;

namespace CrisesControl.Infrastructure.Services;

public class IncidentService : IIncidentService
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IActiveIncidentRepository _activeIncidentRepository;

    public IncidentService(IIncidentRepository incidentRepository,
        IMessageRepository messageRepository,
        IActiveIncidentRepository activeIncidentRepository)
    {
        _incidentRepository = incidentRepository;
        _messageRepository = messageRepository;
        _activeIncidentRepository = activeIncidentRepository;
    }

    public async Task InitiateIncident(IncidentActivation activation, IncidentSubset incidentSubset, CancellationToken cancellationToken = default)
    {
        await _incidentRepository.AddIncidentActivation(activation, cancellationToken);

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
                await _messageRepository.CreateMessageMethod(0, method, activation.IncidentActivationId);
                if (pushMethodId == method)
                    pushAdded = true;
            }

            if (incidentSubset.TrackUser && !pushAdded)
            {
                await _messageRepository.CreateMessageMethod(0, pushMethodId,
                    activation.IncidentActivationId);
            }
        }

        if (incidentSubset.UsersToNotify.Length > 0)
        {
            await _messageRepository.AddUserToNotify(0, incidentSubset.UsersToNotify,
                activation.IncidentActivationId);
        }

        if (incidentSubset.MultiResponse)
        {
            await _messageRepository.SaveActiveMessageResponse(0, incidentSubset.AckOptions,
                activation.IncidentActivationId);
        }

        //TODO: Clarify nominated keyholders

        await _activeIncidentRepository.ProcessImpactedLocation(incidentSubset.ImpactedLocationIds,
            activation.IncidentActivationId, activation.CompanyId, "INITIATE");

        await _activeIncidentRepository.ProcessAffectedLocation(incidentSubset.AffectedLocations,
            activation.IncidentActivationId,
            activation.CompanyId, "INITIATE");
    }

    public Task InitiateAndLaunchIncident(IncidentActivation activation, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}