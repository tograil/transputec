using AutoMapper;
using CrisesControl.Api.Application.Commands.App.AppHome;
using CrisesControl.Api.Application.Commands.App.CaptureUserLocation;
using CrisesControl.Api.Application.Commands.App.CCPhoneNumbers;
using CrisesControl.Api.Application.Commands.App.CheckUserLocation;
using CrisesControl.Api.Application.Commands.App.GetAppLanguage;
using CrisesControl.Api.Application.Commands.App.GetLanguage;
using CrisesControl.Api.Application.Commands.App.GetPrivacyPolicy;
using CrisesControl.Api.Application.Commands.App.GetTnC;
using CrisesControl.Api.Application.Commands.App.ReferToFriend;
using CrisesControl.Api.Application.Commands.App.SendFeedback;
using CrisesControl.Api.Application.Commands.App.UpdateDevice;
using CrisesControl.Api.Application.Commands.App.UpdatePushToken;
using CrisesControl.Api.Application.Commands.App.UpdateTrackMe;
using CrisesControl.Api.Application.Commands.App.ValidatePin;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.App;
using CrisesControl.Core.App.Repositories;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query
{
    public class AppQuery : IAppQuery
    {
        private readonly IAppRepository _appRepository;
        private readonly ILogger<AppQuery> _logger;
        private readonly IDBCommonRepository _DBC;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public AppQuery(IAppRepository appRepository, ILogger<AppQuery> logger, IDBCommonRepository DBC, IMapper mapper, ICurrentUser currentUser)
        {
            this._appRepository = appRepository;
            this._logger = logger;
            this._mapper = mapper;
            this._DBC = DBC;
            this._currentUser = currentUser;
        }

        public async Task<AppHomeResponse> AppHome(AppHomeRequest request)
        {
            try
            {
               string Token = request.Token;
                var home = _appRepository.AppHome(_currentUser.CompanyId, _currentUser.UserId, request.UserDeviceID,Token);
                var result = _mapper.Map<AppHomeReturn>(home);
                var response = new AppHomeResponse();
                if (result!=null)
                {
                    response.Result = result;
                    response.Message = "Data Loaded.";
                }
                else
                {
                    response.Result = null;
                    response.Message = "No record data.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetPrivacyPolicyResponse> GetPrivacyPolicy(GetPrivacyPolicyRequest request)
        {
            try
            {
                var tncText =await  _DBC.LookupWithKey("PRIVACY_POLICY");
                var result = _mapper.Map<string>(tncText);
                var response = new GetPrivacyPolicyResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.Result = result;
                }
                else
                {
                    response.Result = "No record found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Terms and Condition text/link for App
        /// </summary>
        /// <returns></returns>
        public async Task<GetTnCResponse> GetTnC(GetTnCRequest request)
        {
            try
            {
                var tncText =await _DBC.LookupWithKey("TNC"); 
                var result = _mapper.Map<string>(tncText);
                var response = new GetTnCResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.Result = result;                    
                }
                else
                {
                    response.Result = "No record found";                   
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CCPhoneNumbersResponse> CCPhoneNumbers(CCPhoneNumbersRequest request)
        {
            try
            {

                var phoneNumber = await _appRepository.CCPhoneNumbers();
                var result = _mapper.Map<string>(phoneNumber);
                var response = new CCPhoneNumbersResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.PhoneNumber = result;
                }
                else
                {
                    response.PhoneNumber = "No record data.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ReferToFriendResponse> ReferToFriend(ReferToFriendRequest request)
        {
            try
            {

                var home = await _appRepository.ReferToFriend(request.ReferToName,request.ReferToEmail,request.ReferMessage,request.UserEmail,_currentUser.UserId);
                var result = _mapper.Map<string>(home);
                var response = new ReferToFriendResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.Message = result;
                }
                else
                {
                    response.Message = "No record data.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SendFeedbackResponse> SendFeedback(SendFeedbackRequest request)
        {
            try
            {
                
                var home = await _appRepository.SendFeedback(request.DeviceType.ToDeviceString(),request.DeviceOS,request.UserEmail,request.DeviceModel,request.FeedbackMessage);
                var result = _mapper.Map<string>(home);
                var response = new SendFeedbackResponse();
                if (!string.IsNullOrEmpty(result))
                {                    
                    response.Message = result;
                }
                else
                {                   
                    response.Message = "No record data.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateDeviceResponse> UpdateDevice(UpdateDeviceRequest request)
        {
            try
            {

                var device = await _appRepository.UpdateDevice(request.IsSirenOn, request.OverrideSilent,  request.SoundFile, request.UpdateType.ToUString(),request.Language,request.DeviceSerial,_currentUser.CompanyId,_currentUser.UserId);
                var result = _mapper.Map<string>(device);
                var response = new UpdateDeviceResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.Message = result;
                }
                else
                {
                    response.Message = "No record data.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ValidatePinResponse> ValidatePin(ValidatePinRequest request)
        {
            try
            {
                int ValidPin = 12345;
                var response = new ValidatePinResponse();
                if (request.PinNumber == ValidPin)
                {
                    response.PinExpire = 10;
                }
                else
                {
                  
                    response.Message = "Wrong pin entered, please try again";
                   
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CaptureUserLocationResponse> CaptureUserLocation(CaptureUserLocationRequest request)
        {
            try
            {

                var userLocation = await _appRepository.CaptureUserLocation(request.UserLocations,  _currentUser.UserId,request.UserDeviceId, _currentUser.CompanyId,_currentUser.TimeZone);
                var result = _mapper.Map<bool>(userLocation);
                var response = new CaptureUserLocationResponse();
                if (result)
                {
                    response.Result = result;
                    response.Message = "User Location Captured";
                }
                else
                {
                    response.Result = false;
                    response.Message = "No record data.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CheckUserLocationResponse> CheckUserLocation(CheckUserLocationRequest request)
        {
            try
            {

                var userLocation = await _appRepository.GetUserLocationsList(request.UserDeviceID,request.Length,request.Action);
                var result = _mapper.Map<List<UserLocation>>(userLocation);
                var response = new CheckUserLocationResponse();
                if (result!=null)
                {
                    response.Result = result;
                    response.Message = "User Location Checked";
                }
                else
                {
                    response.Result = null;
                    response.Message = "No record data.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetLanguageResponse> GetLanguage(GetLanguageRequest request)
        {
            try
            {

                var userLocation = await _appRepository.GetLanguage(request.Locale);
                var result = _mapper.Map<List<LanguageItem>>(userLocation);
                var response = new GetLanguageResponse();
                if (result != null)
                {
                    response.Result = result;
                    response.Message = "Loaguages Loaded";
                }
                else
                {
                    response.Result = null;
                    response.Message = "No record data.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateTrackMeResponse> UpdateTrackMe(UpdateTrackMeRequest request)
        {
            try
            {

                var trackMe = await _appRepository.UpdateTrackMe(request.Enabled,request.TrackType,request.ActiveIncidentID, request.UserId,request.UserDeviceID,_currentUser.CompanyId,_currentUser.TimeZone,request.Latitude,request.Longitude);
                var result = _mapper.Map<bool>(trackMe);
                var response = new UpdateTrackMeResponse();
                if (result)
                {
                    response.Result = result;
                    response.Message = "Updated";
                }
                else
                {
                    response.Result = false;
                    response.Message = "No record data.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<GetAppLanguageResponse> GetAppLanguage(GetAppLanguageRequest request)
        {
            try
            {

                var userLocation = await _appRepository.GetAppLanguage();
                var result = _mapper.Map<List<AppLanguageList>>(userLocation);
                var response = new GetAppLanguageResponse();
                if (result != null)
                {
                    response.Result = result;
                    response.Message = "Languages Loaded";
                }
                else
                {
                    response.Result = null;
                    response.Message = "No record data.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdatePushTokenResponse> UpdatePushToken(UpdatePushTokenRequest request)
        {
            try
            {

                var pushToken = await _appRepository.UpdatePushToken(request.UserDeviceId,request.PushDeviceId);
                var result = _mapper.Map<bool>(pushToken);
                var response = new UpdatePushTokenResponse();
                if (result)
                {
                    response.Result = result;
                    response.Message = "Languages Loaded";
                }
                else
                {
                    response.Result = false;
                    response.Message = "No record data.";
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
