using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetInvSchedule
{
    public class GetInvScheduleHandler : IRequestHandler<GetInvScheduleRequest, GetInvScheduleResponse>
    {
        private readonly GetInvScheduleValidator _getInvScheduleValidator;
        private readonly IBillingQuery _billingQuery;
        private readonly IMapper _mapper;
        private readonly ILogger<GetInvScheduleHandler> _logger;
        public GetInvScheduleHandler(GetInvScheduleValidator getInvScheduleValidator, ILogger<GetInvScheduleHandler> logger, IMapper mapper, IBillingQuery billingRepository)
        {
            _billingQuery = billingRepository;
            _mapper = mapper;
            _logger = logger;
            _getInvScheduleValidator = getInvScheduleValidator;
        }
        public async Task<GetInvScheduleResponse> Handle(GetInvScheduleRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetInvScheduleRequest));
            await _getInvScheduleValidator.ValidateAndThrowAsync(request,cancellationToken);
            var response = await _billingQuery.GetInvSchedule(request);                      
            return response;
        }
    }
}
