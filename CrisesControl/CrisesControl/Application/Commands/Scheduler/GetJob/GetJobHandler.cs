using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Scheduler.GetJob
{
    public class GetJobHandler : IRequestHandler<GetJobRequest, GetJobResponse>
    {
        private readonly ISchedulerQuery _schedulerQuery;
        private readonly IMapper _mapper;
        public GetJobHandler(ISchedulerQuery schedulerQuery, IMapper mapper)
        {
            this._mapper = mapper;
            this._schedulerQuery = schedulerQuery;
        }

        public async Task<GetJobResponse> Handle(GetJobRequest request, CancellationToken cancellationToken)
        {
            var result = await _schedulerQuery.GetJob(request);
            return result;
        }
    }
}
