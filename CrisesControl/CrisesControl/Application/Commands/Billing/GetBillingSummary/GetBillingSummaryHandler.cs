using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetBillingSummary
{
    public class GetBillingSummaryHandler : IRequestHandler<GetBillingSummaryRequest, GetBillingSummaryResponse>
    {

        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;
        

        public GetBillingSummaryHandler(IBillingRepository billingRepository, IMapper mapper)
        {
            _billingRepository = billingRepository;
            _mapper = mapper;
        }

        public async Task<GetBillingSummaryResponse> Handle(GetBillingSummaryRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetBillingSummaryRequest));

            var billingSummary = await _billingRepository.GetBillingSummary(request.OutUserCompanyId, request.ChkUserId);
            var result = _mapper.Map<BillingSummaryModel>(billingSummary);
            var response = new GetBillingSummaryResponse();
            response.ActiveUserCount = result.ActiveUserCount;
            response.AdminCount = result.AdminCount;
            response.Anniversary = result.Anniversary;
            response.AssetSize = result.AssetSize;
            response.AudioCount = result.AudioCount;
            response.ContractStartDate = result.ContractStartDate;
            response.CreditBalance = result.CreditBalance;
            response.CreditLimit = result.CreditLimit;
            response.DocsCount = result.DocsCount;
            response.ErrorCode = result.ErrorCode;
            response.ErrorId = result.ErrorId;
            response.KeyHolderCount = result.KeyHolderCount;
            response.Message = result.Message;
            response.MinimumBalance = result.MinimumBalance;
            response.MonthEmailMessage = result.MonthEmailMessage;
            response.MonthPhoneMessage = result.MonthPhoneMessage;
            response.MonthPushMessage = result.MonthPushMessage;
            response.MonthTextMessage = result.MonthTextMessage;
            response.PaidServices = result.PaidServices;
            response.PendingUserCount = result.PendingUserCount;
            response.StaffCount = result.StaffCount;
            response.StorageinGB = result.StorageinGB;
            response.StorageLimit = result.StorageLimit;
            response.StorageUsed = result.StorageUsed;
            response.TotalEmailMessage = result.TotalEmailMessage;
            response.TotalPhoneMessage = result.TotalPhoneMessage;
            response.TotalPushMessage = result.TotalPushMessage;
            response.TotalTextMessage = result.TotalTextMessage;
            response.UserLimit = result.UserLimit;
            response.VideoCount = result.VideoCount;
            return response;
        }
    }
}
