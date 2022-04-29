﻿using AutoMapper;
using CrisesControl.Api.Application.Commands.Scheduler.GetAllJobs;
using CrisesControl.Api.Application.Commands.Scheduler.GetJob;
using CrisesControl.Core.Models;
using CrisesControl.Core.Scheduler;
using CrisesControl.Core.Scheduler.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class SchedulerQuery : ISchedulerQuery
    {
        public readonly ISchedulerRepository _schedulerRepository;
        public readonly IMapper _mapper;
        public readonly ILogger<SchedulerQuery> _logger;

        public SchedulerQuery(ISchedulerRepository schudulerRepository, ILogger<SchedulerQuery> logger, IMapper mapper)
        {
            this._schedulerRepository = schudulerRepository;
            this._logger= logger;
            this._mapper=mapper;
        }
        public async Task<GetAllJobsResponse> GetAllJobs(GetAllJobsRequest request)
        {
            var jobs = await _schedulerRepository.GetAllJobs(request.CompanyID, request.UserID);

            var response = _mapper.Map<List<JobSchedulerVM>>(jobs);
            var result = new GetAllJobsResponse();
            result.Data = response;
            result.ErrorCode = "200";
            return result;
        }

        public async Task<GetJobResponse> GetJob(GetJobRequest request)
        {
            var jobs = await _schedulerRepository.GetJob(request.CompanyID, request.JobId);
           
            var response = _mapper.Map<List<JobSchedulerVM>>(jobs);
            var result = new GetJobResponse();
            result.Data = response;
            result.ErrorCode = "200";
            return result;
        }
    }
}
