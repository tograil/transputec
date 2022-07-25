using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using MediatR;
using System.Net;
using Object = CrisesControl.Core.Models.Object;

namespace CrisesControl.Api.Application.Commands.Users.UpdateProfile
{
    public class UpdateProfileHandler : IRequestHandler<UpdateProfileRequest, UpdateProfileResponse>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<UpdateProfileHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly string timeZoneId = "GMT Standard Time";
   
        public UpdateProfileHandler(ICurrentUser currentUser, IMapper mapper, ILogger<UpdateProfileHandler> logger,
        IUserRepository userRepository, ICompanyParametersRepository companyParRepository, CrisesControlContext context)
        {
            this._currentUser = currentUser;
            this._logger = logger;
            this._userRepository = userRepository;
            this._mapper = mapper;
             
        }

        public async Task<UpdateProfileResponse> Handle(UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateProfileRequest));

            try
            {
                User user = await _userRepository.GetUserById(request.UserId) ;
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(request.FirstName))
                        user.FirstName = request.FirstName;
                    if (!string.IsNullOrEmpty(request.LastName))
                        user.LastName = request.LastName;
                    if (!string.IsNullOrEmpty(request.Isdcode))
                        user.Isdcode = request.Isdcode;
                    if (!string.IsNullOrEmpty(request.MobileNo))
                        user.MobileNo = request.MobileNo.FixMobileZero();
                    if (!string.IsNullOrEmpty(request.Llisdcode))
                        user.Llisdcode = request.Llisdcode;
                    if (!string.IsNullOrEmpty(request.Landline))
                        user.Landline = request.Landline.FixMobileZero();
                    if (!string.IsNullOrEmpty(request.UserLanguage))
                        user.UserLanguage = request.UserLanguage;
                    if (!string.IsNullOrEmpty(request.UserPhoto))
                        user.UserPhoto = request.UserPhoto;
                    if (request.TimeZoneID > 0)
                        user.TimezoneId = request.TimeZoneID;
                    user.UpdatedBy = request.UserId;
                    user.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);

                    var Id = await _userRepository.UpdateProfile(user);
                    _mapper.Map<UpdateProfileRequest, User>(request);
                   await _userRepository.CreateUserSearch(user.UserId, user.FirstName, user.LastName, user.Isdcode, user.MobileNo, user.PrimaryEmail, request.CompanyId);

                    string userchannelallowed = await _userRepository.GetCompanyParameter(KeyType.ALLOWCHANGECHANNELUSER.ToDbKeyString(), request.CompanyId);
                    if (userchannelallowed == true.ToString())
                    {
                        //Adding Ping Methods
                        if (request.PingMethod != null)
                        {
                            if (request.PingMethod.Length > 0)
                                _userRepository.UserCommsMethods(user.UserId, MethodType.Ping.ToDbMethodString(), request.PingMethod, _currentUser.UserId, request.CompanyId, timeZoneId);
                        }

                        //Adding Incident Methods
                        if (request.IncidentMethod != null)
                        {
                            if (request.IncidentMethod.Length > 0)
                                _userRepository.UserCommsMethods(user.UserId, MethodType.Incident.ToDbMethodString(), request.IncidentMethod, _currentUser.UserId, request.CompanyId, timeZoneId);
                        }
                    }


                    _userRepository.CreateSMSTriggerRight(request.CompanyId, user.UserId, user.UserRole ?? string.Empty, true, user.Isdcode, user.MobileNo, true);

                    string comppriority = await _userRepository.GetCompanyParameter(KeyType.ALLOWCHANNELPRIORITY.ToDbKeyString(), request.CompanyId);
                    string userpriority = await _userRepository.GetCompanyParameter(KeyType.ALLOWCHANGEPRIORITYUSER.ToDbKeyString(), request.CompanyId);

                    if (comppriority == true.ToString() && userpriority == true.ToString())
                    {
                        if (request.CommsMethod != null)
                            _userRepository.UserCommsPriority(user.UserId, request.CommsMethod, _currentUser.UserId, _currentUser.CompanyId, cancellationToken);
                    }

                    var RegUserInfo = _userRepository.GetRegisteredUserInfo(request.CompanyId,_currentUser.UserId);

                    if (RegUserInfo != null)
                    {
                        return new UpdateProfileResponse()
                        {
                            userId = Id,
                            ErrorCode = HttpStatusCode.OK,
                            Message = "User Profile has been updated"

                        };
                    }
                }
                return new UpdateProfileResponse()
                    {
                        userId = 0,
                        ErrorCode = HttpStatusCode.NotFound,
                        Message = "NOT Found"

                    };
                
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                          ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                throw new UserNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
            }

        }
    }
}
