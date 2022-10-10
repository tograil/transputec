using System.Data.SqlTypes;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Incidents.Services;
using Quartz;
using CrisesControl.Core.Jobs.Services;
using CrisesControl.SharedKernel.Utils;
using MongoDB.Bson.IO;

namespace CrisesControl.Scheduler.Jobs;

public class InitiateIncidentJob : IJob
{
    private readonly ILogger<InitiateIncidentJob> _logger;
    private readonly IIncidentRepository _incidentRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IIncidentService _incidentService;

    public InitiateIncidentJob(ILogger<InitiateIncidentJob> logger,
        IIncidentRepository incidentRepository,
        ICompanyRepository companyRepository,
        IIncidentService incidentService)
    {
        _logger = logger;
        _incidentRepository = incidentRepository;
        _companyRepository = companyRepository;
        _incidentService = incidentService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Started incident job");

        var request = Newtonsoft.Json.JsonConvert
            .DeserializeObject<InitiateIncidentModel>(context.JobDetail.JobDataMap.GetString("command"));

        var incidentToVerify = await _incidentRepository.GetIncident(request.CompanyId, request.IncidentId);

        if (incidentToVerify == null)
        {
            var simulationText = string.Empty;
            if (request.LaunchMode == 4)
            {
                simulationText = await _companyRepository.GetCompanyParameter("INCIDENT_SIMULATION_TEXT", request.CompanyId) +
                    " ";
            }

            var incidentActivation = new IncidentActivation
            {
                Name = (simulationText + incidentToVerify.Name).Trim(),
                IncidentIcon = incidentToVerify.IncidentIcon,
                CompanyId = request.CompanyId,
                IncidentId = request.IncidentId,
                IncidentDescription = request.Description.Trim(),
                Severity = request.Severity,
                ImpactedLocationId = request.ImpactedLocationId.FirstOrDefault(),
                InitiatedOn = DateTime.Now.GetDateTimeOffset(),
                InitiatedBy = request.CurrentUserId,
                LaunchedOn = (DateTime)SqlDateTime.Null,
                LaunchedBy = 0,
                Status = request.Source == "JOB" ? 10 : 1,
                TrackUser = request.TrackUser,
                SilentMessage = request.SilentMessage,
                CreatedBy = request.CurrentUserId,
                CreatedOn = DateTime.Now.GetDateTimeOffset(),
                DeactivatedOn = (DateTime)SqlDateTime.Null,
                ClosedOn = (DateTime)SqlDateTime.Null,
                UpdatedBy = request.CurrentUserId,
                UpdatedOn = DateTime.Now.GetDateTimeOffset(),
                AssetId = request.AudioAssetId,
                HasTask = (bool)incidentToVerify.HasTask,
                LaunchMode = request.LaunchMode,
                SocialHandle = string.Join(",", request.SocialHandle),
                CascadePlanId = request.CascadePlanID
            };

            var incidentSubset = new IncidentSubset
            {
                AckOptions = request.AckOptions,
                MessageMethod = request.MessageMethod,
                MultiResponse = request.MultiResponse,
                TrackUser = request.TrackUser,
                UsersToNotify = request.UsersToNotify,
                ImpactedLocationIds = request.ImpactedLocationId,
                AffectedLocations = request.AffectedLocations
            };

            _incidentService.InitiateIncident(incidentActivation, incidentSubset);


            _logger.LogInformation("Incident initiated");
        }
    }
}