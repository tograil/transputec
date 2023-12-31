﻿using System.Data.SqlTypes;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.Jobs;
using CrisesControl.Core.Jobs.Repositories;
using CrisesControl.Core.Jobs.Services;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using MediatR;
using Newtonsoft.Json;

namespace CrisesControl.Api.Application.Commands.Scheduler.AddJob;

public class AddJobHandler : IRequestHandler<AddJobRequest, AddJobResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AddJobHandler> _logger;
    private readonly IJobRepository _jobRepository;
    private readonly IJobScheduleRepository _jobScheduleRepository;
    private readonly IScheduleService _scheduleService;

    public AddJobHandler(ICurrentUser currentUser,
        ILogger<AddJobHandler> logger,
        IJobRepository jobRepository,
        IJobScheduleRepository jobScheduleRepository,
        IScheduleService scheduleService)
    {
        _currentUser = currentUser;
        _logger = logger;
        _jobRepository = jobRepository;
        _jobScheduleRepository = jobScheduleRepository;
        _scheduleService = scheduleService;
    }

    public async Task<AddJobResponse> Handle(AddJobRequest request, CancellationToken cancellationToken)
    {
        var job = new Job
        {
            JobType = request.JobType,
            JobName = request.JobType.Length > 50 ? request.JobType[..50] : request.JobType,
            JobDescription = request.JobDescription.Length > 500 ? request.JobDescription[..500] : request.JobDescription,
            CommandLine = request.CommandLine,
            CommandLineParams = request.CommandLineParams,
            ActionType = request.ActionType,
            NextRunDate = SqlDateTime.MinValue.Value,
            NextRunTime = string.Empty,
            IsEnabled = request.IsEnabled,
            Locked = false,
            LockedBy = "NEWSCHEDULE",
            JobIncidentId = request.JobIncidentId,
            LastRunDateTime = SqlDateTime.MinValue.Value,
            CreatedDate = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
            CreatedBy = _currentUser.UserId,
            UpdatedDate = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
            UpdatedBy = _currentUser.UserId,
            CompanyId = _currentUser.CompanyId
        };

        var jobId = await _jobRepository.AddJob(job);

        var jobSchedule = new JobSchedule
        {
            JobId = jobId,
            FrequencyType = request.FrequencyType,
            FrequencyInterval = request.FrequencyInterval,
            FrequencySubDayType = request.FrequencySubDayType,
            FrequencySubDayInterval = request.FrequencySubDayInterval,
            RecurrenceFactor = request.RecurrenceFactor,
            ActiveStartDate = request.ActiveStartDate.AddSeconds(TimeSpan.Parse(request.ActiveStartTime).TotalSeconds),
            ActiveStartTime = request.ActiveStartTime,
            ActiveEndDate = request.ActiveEndDate.AddSeconds(TimeSpan.Parse(request.ActiveEndTime).TotalSeconds),
            ActiveEndTime = request.ActiveEndTime,
            NextRunDate = SqlDateTime.MinValue.Value,
            NextRunTime = string.Empty,
            IsActive = request.IsActive,
            CreatedDate = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
            CreatedBy = _currentUser.UserId,
            UpdatedDate = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
            UpdatedBy = _currentUser.UserId
        };

        var jobScheduleId = await _jobScheduleRepository.AddJobSchedule(jobSchedule);

        var command = JobCommandConstants.ParseCommand(request.CommandLine);

        var commandParameters = command switch
        {
            JobType.InitiateIncident => JsonConvert.DeserializeObject<InitiateIncidentModel>(request.CommandLineParams) as CcBase,
            JobType.InitiateAndLaunchIncident => JsonConvert.DeserializeObject<InitiateIncidentModel>(request.CommandLineParams),
            JobType.PingMessage => JsonConvert.DeserializeObject<PingMessageModel>(request.CommandLineParams),
            _ => throw new ArgumentOutOfRangeException()
        };

        var jobQueue = new JobQueueData
        {
            JobData = job,
            JobScheduleId = jobScheduleId,
            FrequencyType = JobCommandConstants.ParseFrequencyType(request.FrequencyType),
            FrequencyInterval = request.FrequencyInterval,
            JobSubDayType = JobCommandConstants.ParseJobSubDayType(request.FrequencySubDayType),
            FrequencySubInterval = request.FrequencySubDayInterval,
            JobType = command,
            JobModel = commandParameters,
            TimeZone = _currentUser.TimeZone
        };

        await _scheduleService.ScheduleIncident(jobQueue);

        return new AddJobResponse
        {
            Success = true,
            JobId = jobId,
            JobScheduleId = jobScheduleId
        };
    }
}