using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Scheduler.GetAllJobs
{
    public class GetAllJobsHandler : IRequestHandler<GetAllJobsRequest, GetAllJobsResponse>
    {
        private readonly ISchedulerQuery _schedulerQuery;
        private readonly IMapper _mapper;
        public GetAllJobsHandler(ISchedulerQuery schedulerQuery, IMapper mapper)
        {
            this._mapper = mapper;
            this._schedulerQuery= schedulerQuery;
        }
        public async Task<GetAllJobsResponse> Handle(GetAllJobsRequest request, CancellationToken cancellationToken)
        {
            var result = await _schedulerQuery.GetAllJobs(request);
            return result;
        }
    }
}
