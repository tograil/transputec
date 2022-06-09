using AutoMapper;
using CrisesControl.Api.Application.Commands.Register.ActivateCompany;
using CrisesControl.Api.Application.Commands.Register.CheckAppDownloaded;
using CrisesControl.Api.Application.Commands.Register.CheckCustomer;
using CrisesControl.Api.Application.Commands.Register.CreateSampleIncident;
using CrisesControl.Api.Application.Commands.Register.DeleteTempRegistration;
using CrisesControl.Api.Application.Commands.Register.GetTempRegistration;
using CrisesControl.Api.Application.Commands.Register.SetupCompleted;
using CrisesControl.Api.Application.Commands.Register.TempRegister;
using CrisesControl.Api.Application.Commands.Register.UpgradeRequest;
using CrisesControl.Api.Application.Commands.Register.ValidateMobile;
using CrisesControl.Api.Application.Commands.Register.ValidateUserEmail;
using CrisesControl.Api.Application.Commands.Register.VerifyTempRegistration;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Incidents.Repositories;
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
        private readonly IIncidentRepository _incidentRepository;
        public RegisterQuery(IRegisterRepository registerRepository, IMapper mapper, IIncidentRepository incidentRepository,
        ILogger<RegisterQuery> logger, ICompanyRepository companyRepository, ICurrentUser currentUser)
        {
            this. _mapper = mapper;
            this._registerRepository = registerRepository;
            this._logger = logger;
            this._companyRepository=companyRepository;
            this._currentUser=currentUser;
            this._incidentRepository=incidentRepository;

        }

        public async Task<ActivateCompanyResponse> ActivateCompany(ActivateCompanyRequest request)
        {
            var status =await _registerRepository.ActivateCompany(request.UserId,request.ActivationKey,request.IPAddress,request.SalesSource);
            var result = _mapper.Map<ActivateCompanyResponse>(status);

            if (status)
            {
                result.Activated= true;
            }
            else
            {
                result.Activated = false;
                result.Message = "Invalid activation key entered, try again.";
            }
            return result;
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
       public async Task<CheckAppDownloadResponse> CheckAppDownload(CheckAppDownloadRequest request)
        {
            try
            {
                var device = await _registerRepository.GetUserDeviceByUserId(request.UserId);
                var response = _mapper.Map<CheckAppDownloadResponse>(device);

                if (device != null)
                {

                    response.Data = device;
                    response.Message = "DOWNLOADED";
                }
                else
                {
                    response.Data = device;
                    response.Message = "NOT_DOWNLOAD";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<CreateSampleIncidentResponse> CreateSampleIncident(CreateSampleIncidentRequest request)
        {
            var CompanyStatus = _companyRepository.GetCompanyByID(request.CompanyId);
            if (CompanyStatus != null)
            {

                int incident_id = await _incidentRepository.ActivateSampleIncident(_currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);

                
                await _incidentRepository.CreateSOSIncident(_currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<CreateSampleIncidentResponse>(incident_id);
                var response = new CreateSampleIncidentResponse();
                response.result = result.result;
                response.Message = "Company has been Found";
                return response;
            }
            throw new RegisterNotFoundException(0,_currentUser.UserId);
        }

        public async  Task<DeleteTempRegistrationResponse> DeleteTempRegistration(DeleteTempRegistrationRequest request)
        {
            var delete = await _registerRepository.GetRegistrationByUniqueReference(request.UniqueReference);
            var response = new DeleteTempRegistrationResponse();
            if (delete != null)
            {
                var result = await _registerRepository.DeleteTempRegistration(delete);
                response.Result = result;
                response.Message = "Deleted";
                return response;
            }
            throw new RegisterNotFoundException(0, _currentUser.UserId);
        }

        public async Task<GetTempRegistrationReponse> GetTempRegistration(GetTempRegistrationRequest request)
        {
            var temp = await _registerRepository.GetTempRegistration(request.RegId,request.UniqueRef);
            var result = _mapper.Map<GetTempRegistrationReponse>(temp);
            var response = new GetTempRegistrationReponse();
            response.data = result.data;
            return response;
        }

        public async Task<SetupCompletedResponse> SetupCompleted(SetupCompletedRequest request)
        {
            var CompanyStatus = await _companyRepository.GetCompanyByID(_currentUser.CompanyId);
            if (CompanyStatus != null)
            {
                var newReg = _mapper.Map<Company>(CompanyStatus);
                await _incidentRepository.CopyIncidentToCompany(_currentUser.CompanyId, _currentUser.UserId, _currentUser.TimeZone);

                newReg.CompanyProfile = "ON_TRIAL";
                newReg.OnTrial = true;
                var setup =await _registerRepository.SetupCompleted(newReg);

                var response = new SetupCompletedResponse();
                response.Message = "Company Completed";
                response.Result = setup.ToString();
                return response;

            }
            throw new CompanyNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }

        public async Task<TempRegisterResponse> TempRegister(TempRegisterRequest request)
        {
            var reg = await _registerRepository.GetRegistrationDataByEmail(request.Email);
            var response=new TempRegisterResponse();
            if (reg == null)
            {
                Registration RG = new Registration();
                RG.FirstName = request.FirstName;
                RG.LastName = request.LastName;
                RG.Password = request.Password;
                RG.Email = request.Email;
                RG.UniqueReference = Guid.NewGuid().ToString();
                RG.CreatedOn = DateTime.Now.GetDateTimeOffset();
                RG.Status = 1;
                RG.PackagePlanId = request.PackagePlanId;
                RG.PaymentMethod = request.PaymentMethod;
                RG.CountryCode = "GBR";
                RG.MobileIsd = "+44";
                RG.CustomerId = request.CustomerId;

                var newReg = _mapper.Map<Registration>(RG);
                var temp=await  _registerRepository.TempRegister(newReg);
                response.Data = temp;
                return response;
            }
            else
            {
                if (!string.IsNullOrEmpty(request.FirstName))
                    reg.FirstName = request.FirstName;

                if (!string.IsNullOrEmpty(request.LastName))
                    reg.LastName = request.LastName;

                if (!string.IsNullOrEmpty(request.Password))
                    reg.Password = request.Password;

                if (!string.IsNullOrEmpty(request.MobileISD))
                    reg.MobileIsd = request.MobileISD;

                if (!string.IsNullOrEmpty(request.MobileNo))
                    reg.MobileNo = request.MobileNo;

                if (!string.IsNullOrEmpty(request.Sector))
                    reg.Sector = request.Sector;

                if (request.RegAction == "CHANGE" && !string.IsNullOrEmpty(request.NewRegEmail))
                    reg.Email = request.NewRegEmail;

                if (request.RegAction != "MOBILECHANGE")
                {
                    reg.CompanyName = !string.IsNullOrEmpty(request.CompanyName) ? request.CompanyName : ""; ;
                    reg.CustomerId = !string.IsNullOrEmpty(request.CustomerId) ? request.CustomerId : ""; ;
                    reg.AddressLine1 = !string.IsNullOrEmpty(request.AddressLine1) ? request.AddressLine1 : "";
                    reg.AddressLine2 = !string.IsNullOrEmpty(request.AddressLine2) ? request.AddressLine2 : "";
                    reg.City = !string.IsNullOrEmpty(request.City) ? request.City : "";
                    reg.State = !string.IsNullOrEmpty(request.State) ? request.State : "";
                    reg.Postcode = !string.IsNullOrEmpty(request.Postcode) ? request.Postcode : "";
                    reg.CountryCode = !string.IsNullOrEmpty(request.CountryCode) ? request.CountryCode : "GBR";
                }

                if (!string.IsNullOrEmpty(request.VerificationCode))
                {
                    reg.VerificationCode = request.VerificationCode;
                    reg.VerficationExpire = DateTime.Now.AddMinutes(15);
                }
                reg.Status = request.Status;

                if (request.Status != 2)
                    reg.UniqueReference = Guid.NewGuid().ToString();

                if (reg.Status == 1 && request.RegAction != "CHANGE")
                {
                    reg.CreatedOn = DateTime.Now.GetDateTimeOffset();
                }
               
            }

            if (reg.Status == 1)
            {
               
               // SDE.SendNewRegistration(reg);
            }
            var result = await _registerRepository.TempRegister(reg);
            response.Data = result;
            return response;
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
                Registration registration = await _registerRepository.GetRegistrationByUniqueReference(request.UniqueRef);
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
               

            }
            catch (Exception ex)
            {

                _logger.LogError("Error occured while seeding into a database");
            }
            throw new RegisterNotFoundException(0, _currentUser.UserId);

        }
    }
}
