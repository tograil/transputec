using CrisesControl.Api.Application.Commands.Common;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddCompanyIncident;

public class AddCompanyIncidentHandler : IRequestHandler<AddCompanyIncidentRequest, ResultResponse>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ICurrentUser _currentUser;

    public AddCompanyIncidentHandler(IIncidentRepository incidentRepository,
        ICurrentUser currentUser,
        IMessageRepository messageRepository)
    {
        _incidentRepository = incidentRepository;
        _currentUser = currentUser;
        _messageRepository = messageRepository;
    }

    public async Task<ResultResponse> Handle(AddCompanyIncidentRequest request, CancellationToken cancellationToken)
    {
        if (!await _incidentRepository.CheckDuplicate(_currentUser.CompanyId, request.Name, 0))
        {
            return new ResultResponse(211,string.Empty,"Duplicate Incident.");
        }

        var incident = new Incident
        {
            CompanyId = request.CompanyId,
            IncidentIcon = request.IncidentIcon,
            Name = request.Name,
            Description = request.Description,
            PlanAssetId = request.PlanAssetId,
            IncidentTypeId = request.IncidentTypeId,
            Severity = request.Severity,
            Status = request.Status,
            NumberOfKeyHolders = request.NumberOfKeyHolder,
            AudioAssetId = request.AudioAssetId,
            TrackUser = request.TrackUser,
            SilentMessage = request.SilentMessage,
            CreatedBy = _currentUser.UserId,
            CreatedOn = DateTimeOffset.UtcNow,
            UpdatedBy = _currentUser.UserId,
            IsSos = request.IsSOS,
            CascadePlanId = request.CascadePlanId,
            UpdatedOn = DateTimeOffset.UtcNow
        };

        var incidentId = await _incidentRepository.AddIncident(incident);

        if (incidentId <= 0) return new ResultResponse(110, string.Empty, "No record found.");

        var contacts = request.AddIncidentKeyHldLst.Where(x => x.UserId is not null)
            .Select(x => new IncidentKeyContact
            {
                CompanyId = _currentUser.CompanyId,
                IncidentId = incidentId,
                UserId = x.UserId ?? 0,
                CreatedBy = _currentUser.UserId,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedBy = _currentUser.UserId,
                UpdatedOn = DateTimeOffset.UtcNow
            }).ToArray();

        await _incidentRepository.AddIncidentKeyContacts(contacts);

        await _incidentRepository.ProcessKeyHolders(_currentUser.CompanyId,
            incidentId, _currentUser.UserId, request.KeyHolders);

        await _incidentRepository.SaveIncidentMessageResponse(request.AckOptions, incidentId);

        await _incidentRepository.AddIncidentGroup(incidentId, request.Groups, _currentUser.CompanyId);

        if (request.MessageMethod.Length > 0 && request.CascadePlanId <= 0)
        {
            foreach (var i in request.MessageMethod)
            {
                await _messageRepository.CreateMessageMethod(0, i, 0, incidentId);
            }
        }

        await _incidentRepository.CreateIncidentSegLinks(incidentId, _currentUser.UserId, _currentUser.CompanyId);

        return new ResultResponse(0, string.Empty, "Ok");

    }
}