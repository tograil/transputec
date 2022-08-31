using AutoMapper;
using CrisesControl.Api.Application.Commands.Payments.AddRemoveModule;
using CrisesControl.Api.Application.Commands.Payments.GetCompanyPackageItems;
using CrisesControl.Api.Application.Commands.Payments.GetPackageAddons;
using CrisesControl.Api.Application.Commands.Payments.UpdateCompanyPaymentProfile;
using CrisesControl.Api.Application.Commands.Payments.UpgradeByKey;
using CrisesControl.Api.Application.Commands.Payments.UpgradePackage;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Payments.Repositories;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query
{
    public class PaymentQuery : IPaymentQuery
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentQuery> _logger;
        private readonly IPaging _paging;
        private readonly ICurrentUser _currentUser;
        public PaymentQuery(ICurrentUser currentUser, IPaging paging, ILogger<PaymentQuery> logger, IPaymentRepository paymentRepository)
        {
            this._currentUser = currentUser;
            this._logger = logger;
            this._paging = paging;
            this._paymentRepository = paymentRepository;
        }

        public async Task<AddRemoveModuleResponse> AddRemoveModule(AddRemoveModuleRequest request)
        {
            try
            {
                var module = await _paymentRepository.AddRemoveModule( _currentUser.CompanyId,request.ModuleID,request.ActionValue);
                var response = new AddRemoveModuleResponse();
                if (module)
                {
                    response.Data = module;
                    response.Message = "Module Added";
                }
                else
                {
                    response.Data = false;
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetCompanyPackageItemsResponse> GetCompanyPackageItems(GetCompanyPackageItemsRequest request)
        {
            try
            {
                var profile = await _paymentRepository.GetCompanyPackageItems(_currentUser.CompanyId);
                var response = new GetCompanyPackageItemsResponse();
                if (profile != null)
                {
                    response.Data = profile;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.Data = null;
                    response.Message = "No data found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetPackageAddonsResponse> GetPackageAddons(GetPackageAddonsRequest request)
        {
            try
            {
                var addons = await _paymentRepository.GetPackageAddons(_currentUser.CompanyId);
                var response = new GetPackageAddonsResponse();
                if (addons != null)
                {
                    response.Data = addons;
                }
                else
                {
                    response.Data = null;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateCompanyPaymentProfileResponse> UpdateCompanyPaymentProfile(UpdateCompanyPaymentProfileRequest request)
        {
            try
            {
                var profile = await _paymentRepository.UpdateCompanyPaymentProfile(request.Model,_currentUser.UserId,_currentUser.CompanyId,_currentUser.TimeZone);
                var response = new UpdateCompanyPaymentProfileResponse();
                if (profile != null)
                {
                    response.Data = profile;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.Data = null;
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpgradeByKeyResponse> UpgradeByKey(UpgradeByKeyRequest request)
        {
            const string sub = "SUBSCRIBED";
            var cp = await _paymentRepository.GetCompanyByKey(request.ActivationKey, request.OutUserCompanyId);
            //var result = _mapper.Map<Company>(cp);
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
               
                

                response.CompanyId = intCompany;
                response.Message = "Payment is Upgraded ";
                return response;
            }
            throw new CompanyNotFoundException(_currentUser.CompanyId,_currentUser.UserId);
            

        }

        public async Task<UpgradePackageResponse> UpgradePackage(UpgradePackageRequest request)
        {
            try
            {
                var package = await _paymentRepository.UpgradePackage(request.Model, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
                var response = new UpgradePackageResponse();
                if (package)
                {
                    response.Result = package;
                    response.Message = "Package Upgraded";
                }
                else
                {
                    response.Result = false;
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
