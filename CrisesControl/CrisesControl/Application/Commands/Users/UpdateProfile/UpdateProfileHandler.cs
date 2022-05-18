using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using MediatR;
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
        private readonly CrisesControlContext _context;

        private readonly ICompanyParametersRepository _companyParRepository;
        public UpdateProfileHandler(ICurrentUser currentUser, IMapper mapper, ILogger<UpdateProfileHandler> logger,
        IUserRepository userRepository, ICompanyParametersRepository companyParRepository, CrisesControlContext context)
        {
            this._currentUser = currentUser;
            this._logger = logger;
            this._userRepository = userRepository;
            this._mapper = mapper;
            this._companyParRepository=companyParRepository;
            this._context = context;    
        }

        public async Task<UpdateProfileResponse> Handle(UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            const string key = "ALLOW_CHANGE_CHANNEL_USER";
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
                    if (request.TimezoneId > 0)
                        user.TimezoneId = request.TimezoneId;
                    user.UpdatedBy = request.UserId;
                    user.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);

                    var Id = await _userRepository.UpdateProfile(user);
                    _mapper.Map<UpdateProfileRequest, User>(request);
                    _userRepository.CreateUserSearch(user.UserId, user.FirstName, user.LastName, user.Isdcode, user.MobileNo, user.PrimaryEmail, request.CompanyId);

                    string userchannelallowed = await _userRepository.GetCompanyParameter(key, request.CompanyId);
                    if (userchannelallowed == "true")
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

                    if (comppriority == "true" && userpriority == "true")
                    {
                        if (request.CommsMethod != null)
                            _userRepository.UserCommsPriority(user.UserId, request.CommsMethod, _currentUser.UserId, _currentUser.CompanyId, cancellationToken);
                    }

                    var RegUserInfo = (from Usersval in _context.Set<User>()
                                       where Usersval.CompanyId == request.CompanyId && Usersval.UserId == _currentUser.UserId
                                       select new
                                       {
                                           CompanyId = request.CompanyId,
                                           CurrentUserId = _currentUser.UserId,
                                           UserId = Usersval.UserId,
                                           First_Name = Usersval.FirstName,
                                           Last_Name = Usersval.LastName,
                                           MobileISDCode = Usersval.Isdcode,
                                           MobileNo = Usersval.MobileNo,
                                           LLISDCode = Usersval.Llisdcode,
                                           Landline = request.Landline,
                                           Primary_Email = Usersval.PrimaryEmail,
                                           UserPassword = Usersval.Password,
                                           UserPhoto = Usersval.UserPhoto,
                                           UserRole = Usersval.UserRole,
                                           UniqueGuiID = Usersval.UniqueGuiId,
                                           RegisterUser = Usersval.RegisteredUser,
                                           Status = Usersval.Status,
                                           CommsMethod = (from UC in _context.Set<UserComm>()
                                                          where UC.UserId == Usersval.UserId && UC.Status == 1
                                                          select new
                                                          {
                                                              MethodId = UC.MethodId,
                                                              MessageType = UC.MessageType,
                                                              UC.Priority
                                                          }),
                                           SecGroup = (from SG in _context.Set<SecurityGroup>()
                                                       join USG in _context.Set<UserSecurityGroup>() on SG.SecurityGroupId equals USG.SecurityGroupId
                                                       where SG.CompanyId == request.CompanyId && USG.UserId == Usersval.UserId
                                                       select new
                                                       {
                                                           SecUserId = USG.UserId,
                                                           SecurityGroupId = SG.SecurityGroupId
                                                       }),
                                           OBJMap = (from UOR in _context.Set<ObjectRelation>()
                                                     join OBM in _context.Set<ObjectMapping>() on UOR.ObjectMappingId equals OBM.ObjectMappingId
                                                     join OBJ in _context.Set<Object>() on OBM.TargetObjectId equals OBJ.ObjectId
                                                     where (OBM.CompanyId == request.CompanyId || OBM.CompanyId == null) && UOR.TargetObjectPrimaryId == Usersval.UserId
                                                     select new
                                                     {
                                                         ObjectTableName = OBJ.ObjectTableName,
                                                         SourceObjectPrimaryId = UOR.SourceObjectPrimaryId,
                                                         ObjectMappingId = OBM.ObjectMappingId
                                                     }),
                                       });

                    if (RegUserInfo != null)
                    {
                        return new UpdateProfileResponse()
                        {
                            userId = Id,
                            ErrorCode = System.Net.HttpStatusCode.OK,
                            Message = "User Profile has been updated"

                        };
                    }
                }
                return new UpdateProfileResponse()
                    {
                        userId = 0,
                        ErrorCode = System.Net.HttpStatusCode.NotFound,
                        Message = "NOT Found"

                    };
                
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                          ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return null;
            }

        }
    }
}
