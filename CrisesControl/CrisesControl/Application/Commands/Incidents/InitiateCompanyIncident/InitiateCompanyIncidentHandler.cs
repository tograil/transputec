using System.Data.SqlTypes;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.SharedKernel.Utils;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.InitiateCompanyIncident;

public class InitiateCompanyIncidentHandler : IRequestHandler<InitiateCompanyIncidentRequest, InitiateCompanyIncidentResponse>
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly ICurrentUser _currentUser;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    public InitiateCompanyIncidentHandler(IIncidentRepository incidentRepository,
        ICurrentUser currentUser,
        ICompanyRepository companyRepository,
        IMessageRepository messageRepository,
        IUserRepository userRepository)
    {
        _incidentRepository = incidentRepository;
        _currentUser = currentUser;
        _companyRepository = companyRepository;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

    public async Task<InitiateCompanyIncidentResponse> Handle(InitiateCompanyIncidentRequest request,
        CancellationToken cancellationToken)
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
                LaunchedOn = (DateTime)SqlDateTime.Null,
                LaunchedBy = 0,
                Status = request.Source == "JOB" ? 10 : 1,
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

            if (request.MessageMethod.Length > 0)
            {
                var pushAdded = false;
                var pushMethodId = 1;
                if (request.TrackUser)
                {
                    pushMethodId = _messageRepository.GetPushMethodId();
                }

                foreach (var method in request.MessageMethod)
                {
                    await _messageRepository.CreateMessageMethod(0, method, incidentActivation.IncidentActivationId);
                    if (pushMethodId == method)
                        pushAdded = true;
                }

                if (request.TrackUser && !pushAdded)
                {
                    await _messageRepository.CreateMessageMethod(0, pushMethodId,
                        incidentActivation.IncidentActivationId);
                }
            }

            if (request.UsersToNotify.Length > 0)
            {
                await _messageRepository.AddUserToNotify(0, request.UsersToNotify,
                    incidentActivation.IncidentActivationId);
            }

            if (request.MultiResponse)
            {
                await _messageRepository.SaveActiveMessageResponse(0, request.AckOptions,
                    incidentActivation.IncidentActivationId);
            }

            var incidentToReturn = await _incidentRepository.GetIncidentActivation(_currentUser.CompanyId,
                incidentActivation.IncidentActivationId);

            if (incidentToReturn is not null)
            {
                var initiatedUser = await _userRepository.GetUserById(incidentToReturn.InitiatedBy ?? 0);
                var launchedUser = await _userRepository.GetUserById(incidentToReturn.LaunchedBy ?? 0);
                var deactivatedUser = await _userRepository.GetUserById(incidentToReturn.DeactivatedBy ?? 0);
                var closedUser = await _userRepository.GetUserById(incidentToReturn.ClosedBy ?? 0);

                var result = new InitiateCompanyIncidentResponse
                {
                    Name = incidentToReturn.Name!,
                    Icon = incidentToReturn.IncidentIcon!,
                    CompanyId = incidentToReturn.CompanyId,
                    Description = incidentToReturn.IncidentDescription!,
                    IncidentActivationId = incidentToReturn.IncidentActivationId,
                    Severity = incidentToReturn.Severity,
                    ImpactedLocationId = incidentToReturn.ImpactedLocationId,
                    IncidentId = incidentToReturn.IncidentId,
                    InitiatedOn = incidentToReturn.InitiatedOn,
                    InitiatedBy = incidentToReturn.InitiatedBy ?? 0,
                    InitiatedByName = initiatedUser is not null
                        ? new UserFullName { Firstname = initiatedUser.FirstName, Lastname = initiatedUser.LastName }
                        : new UserFullName(),
                    LaunchedOn = incidentToReturn.LaunchedOn,
                    LaunchedBy = incidentToReturn.LaunchedBy ?? 0,
                    LaunchedByName = launchedUser is not null
                        ? new UserFullName { Lastname = launchedUser.LastName, Firstname = launchedUser.FirstName }
                        : new UserFullName(),
                    DeactivatedOn = incidentToReturn.DeactivatedOn,
                    DeactivatedBy = incidentToReturn.DeactivatedBy ?? 0,
                    DeactivatedByName = deactivatedUser is not null
                        ? new UserFullName { Lastname = deactivatedUser.LastName, Firstname = deactivatedUser.FirstName }
                        : new UserFullName(),
                    ClosedOn = incidentToReturn.ClosedOn,
                    ClosedBy = incidentToReturn.ClosedBy ?? 0,
                    ClosedByName = closedUser is not null
                        ? new UserFullName { Lastname = closedUser.LastName, Firstname = closedUser.FirstName }
                        : new UserFullName(),
                    AssetId = incidentToReturn.AssetId,
                    StatusId = incidentToReturn.Status,
                    HasNotes = incidentToReturn.HasNotes,
                    SocialHandle = incidentToReturn.SocialHandle ?? string.Empty,
                    Status = await _incidentRepository.GetStatusName(incidentToReturn.Status),
                };

                return result;
            }
        }

        return null!;
    }
}