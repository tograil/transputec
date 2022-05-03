using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Scheduler.GetAllJobs
{
    public class GetAllJobsHandler : IRequestHandler<GetAllJobsRequest, GetAllJobsResponse>
    {
        private readonly GetAllJobsValidator _exTriggertValidator;
        private readonly ISchedulerQuery _schedulerQuery;
        private readonly IMapper _mapper;
        public GetAllJobsHandler(ISchedulerQuery schedulerQuery, IMapper mapper)
        {
            this._mapper = mapper;
            this._schedulerQuery= schedulerQuery;
        }
        public async Task<GetAllJobsResponse> Handle(GetAllJobsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAllJobsRequest));

            await _exTriggertValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _schedulerQuery.GetAllJobs(request);
            return result;
        }
    }
}
