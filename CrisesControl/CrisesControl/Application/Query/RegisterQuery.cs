﻿using AutoMapper;
using CrisesControl.Api.Application.Commands.Register.CheckCustomer;
using CrisesControl.Api.Application.Commands.Register.UpgradeRequest;
using CrisesControl.Api.Application.Commands.Register.ValidateMobile;
using CrisesControl.Api.Application.Commands.Register.ValidateUserEmail;
using CrisesControl.Api.Application.Commands.Register.VerifyTempRegistration;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Register.Repositories;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query
{
    public class RegisterQuery : IRegisterQuery
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<RegisterQuery> _logger;
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICurrentUser _currentUser;
        public RegisterQuery(IRegisterRepository registerRepository, IMapper mapper,
        ILogger<RegisterQuery> logger, ICompanyRepository companyRepository, ICurrentUser currentUser)
        {
           this. _mapper = mapper;
            this._registerRepository = registerRepository;
            this._logger = logger;
            this._companyRepository=companyRepository;
        }
        public async Task<CheckCustomerResponse> CheckCustomer(CheckCustomerRequest request)
        {
            var customer = await _registerRepository.CheckCustomer(request.CustomerId);                    
            var result = _mapper.Map<CheckCustomerResponse>(customer);
            if (customer)
            {
                result.Message = "Customer ID already taken";
            }
            else
            {
                result.Message = "No record found.";
            }

            return result;
        }

        public async Task<UpgradeResponse> UpgradeRequest(UpgradeRequest request)
        {
            var status = await _registerRepository.UpgradeRequest(request.CompanyId);
            var result = _mapper.Map<UpgradeResponse>(status);
            if (status)
            {
                result.Message = "Company is upgraded";
                result.Status = status;
            }
            else
            {
                result.Status = status;
                result.Message = "No record found.";
            }

            return result;
        }

        public async Task<VerifyPhoneResponse> ValidateMobile(VerifyPhoneRequest request)
        {
            var phone = await _registerRepository.ValidateMobile(request.Code, request.ISD, request.MobileNo);
            var result = _mapper.Map<VerifyPhoneResponse>(phone);
            if (!string.IsNullOrEmpty(phone))
            {
                result.Message = "Mobile Verified";
            }
            else
            {
                result.Message = "No record found.";
            }

            return result;
        }

        public async Task<ValidateUserEmailResponse> ValidateUserEmail(ValidateUserEmailRequest request)
        {
            var UserInfo = await _registerRepository.ValidateUserEmail(request.uniqueId, request.CompanyId);
            var response= new ValidateUserEmailResponse();
            if (UserInfo!=null)
            {
                if (UserInfo.Status == 1)
                {
                    response.CompanyId = request.CompanyId;
                    response.UserId = UserInfo.UserId;
                    response.Status = UserInfo.Status.ToString();
                   // response.ErrorId = 103;
                    response.Message = "This email id is already verified.";
                }
                else if (UserInfo.Status == 3)
                {
                    response.CompanyId = request.CompanyId;
                    response.UserId = UserInfo.UserId;
                    response.Status = UserInfo.Status.ToString();
                    //response.ErrorId = 148;
                    response.Message = "User do not exist in the system";
                }
                else if (UserInfo.Status == 0)
                {
                    response.CompanyId = request.CompanyId;
                    response.UserId = UserInfo.UserId;
                    response.Status = UserInfo.Status.ToString();
                    //response.ErrorId = 105;
                    response.Message = "User is blocked by administrator";
                }
                else
                {
                    string TimeZoneId = await _companyRepository.GetTimeZone(request.CompanyId);

                    //Assign the user to the default location and depeartment and send the account details
                  await _registerRepository.CreateObjectRelationship(UserInfo.UserId, 0, "Location", request.CompanyId, UserInfo.UserId, TimeZoneId, "ALL");
                    await _registerRepository.CreateObjectRelationship(UserInfo.UserId, 0, "Group", request.CompanyId, UserInfo.UserId, TimeZoneId, "ALL");
                   await _registerRepository.NewUserAccountConfirm(UserInfo.PrimaryEmail, UserInfo.FirstName + " " + UserInfo.LastName, UserInfo.Password, request.CompanyId, request.uniqueId);

                    UserInfo.Status = 1;

                    UserInfo.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    UserInfo.UpdatedBy = UserInfo.UserId;
            

                    response.CompanyId = request.CompanyId;
                    response.UserId = UserInfo.UserId;
                    response.FirstName = UserInfo.FirstName;
                    response.LastName = UserInfo.LastName;
                    response.Email = UserInfo.PrimaryEmail;
                    response.Password = UserInfo.Password;
                    response.Status = UserInfo.Status.ToString();
                    response.Message = "Validated Sucessfully";
                }
                return response;
            }
            throw new CompanyNotFoundException(request.CompanyId, UserInfo.UserId);
        }

        public async Task<VerifyTempRegistrationResponse> VerifyTempRegistration(VerifyTempRegistrationRequest request)
        {
            try
            {
                Registration registration = await _registerRepository.GetRegistrationById(request.UniqueRef);
                if (registration != null)
                {
                    if(registration.Status==1){
                        registration.Status = 2;
                        var result=  await _registerRepository.VerifyTempRegistration(registration);
                        var response = _mapper.Map<VerifyTempRegistrationResponse>(result);
                        response.Message = "Data Loaded";
                        response.RegId = result;
                        return response;
                    }
                  
                                    
                }
                return new VerifyTempRegistrationResponse()
                {
                    Message = "Not Found",
                    RegId = 0
                }
                      ;

            }
            catch (Exception ex)
            {
                throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
                return null;
            }

        }
    }
}
