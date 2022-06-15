using AutoMapper;
using CrisesControl.Api.Application.Commands.Payments.UpgradeByKey;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Payments.Repositories;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query
{
    public class PaymentQuery : IPaymentQuery
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly string _timeZoneId = "GMT Standard Time";
        private readonly ILogger<PaymentQuery> _logger;
        private readonly IPaging _paging;
        private readonly ICurrentUser _currentUser;
        public PaymentQuery(ICurrentUser currentUser, IPaging paging, ILogger<PaymentQuery> logger, IMapper mapper, IPaymentRepository paymentRepository)
        {
            this._currentUser = currentUser;
            this._logger = logger;
            this._paging = paging;
            this._mapper = mapper;
            this._paymentRepository = paymentRepository;
        }
        public async Task<UpgradeByKeyResponse> UpgradeByKey(UpgradeByKeyRequest request)
        {
            const string sub = "SUBSCRIBED";
            var cp = await _paymentRepository.GetCompanyByKey(request.ActivationKey, _currentUser.CompanyId);
            var result = _mapper.Map<Company>(cp);
            var response = new UpgradeByKeyResponse();
            if (cp != null)
            {
                var companyPaymentProfile = cp.CompanyPaymentProfiles?.FirstOrDefault();
                DateTimeOffset dtNow = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);

                cp.CompanyActivation.ActivatedBy = _currentUser.UserId;
                cp.CompanyActivation.ActivatedOn = dtNow;
                cp.CompanyActivation.Status = 1;
                cp.CompanyActivation.Ipaddress = request.IPAddress;
                cp.CompanyActivation.SalesSource = 0;
                cp.Status = 1;

                cp.CompanyProfile = sub;
                cp.OnTrial = await _paymentRepository.OnTrialStatus(sub, false);
                companyPaymentProfile.UpdatedOn = dtNow;
                companyPaymentProfile.UpdatedBy = _currentUser.UserId;

                var intCompany = await _paymentRepository.UpgradeByKey(cp);
               
                

                response.CompanyId = result.CompanyId;
                response.Message = "Payment is Upgraded ";
                return response;
            }
            throw new CompanyNotFoundException(_currentUser.CompanyId,_currentUser.UserId);
            

        }
    }
}
