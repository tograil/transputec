using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Models;
using CrisesControl.SharedKernel.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace CrisesControl.Core.Users.Repositories;

public interface IUserRepository
{
    Task<int> CreateUser(User user, CancellationToken cancellationToken);
    bool EmailExists(string email);
    Task<User?> GetUserById(int id);
    Task<int> UpdateUser(User user, CancellationToken cancellationToken);
    int AddPwdChangeHistory(int userId, string newPassword, string timeZoneId);

    Task CreateUserSearch(int userId, string firstName, string lastName, string isdCode, string mobileNo,
        string primaryEmail, int companyId);
    Task<IEnumerable<User>> GetAllUsers(GetAllUserRequestList userRequest);
    Task<User> GetUser(int companyId, int userId);
    Task<User> DeleteUser(User user, CancellationToken token);
    bool CheckDuplicate(User user);
    Task<LoginInfoReturnModel> GetLoggedInUserInfo(LoginInfo request, CancellationToken cancellationToken);
    Task<User> ReactivateUser(int qureiedUserId, CancellationToken cancellationToken);
    Task<List<GetAllUserDevices>> GetAllUserDeviceList(GetAllUserDeviceRequest request, CancellationToken cancellation);

    Task<ValidateEmailReponseModel> ValidateLoginEmail(string UserName);
    Task<List<UserComm>> GetUserComms(int commsUserId, CancellationToken cancellationToken);
    Task<int> UpdateProfile(User user);
    Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "");
    void CreateSMSTriggerRight(int CompanyId, int UserId, string UserRole, bool SMSTrigger, string ISDCode, string MobileNo, bool Self = false);
    void UserCommsPriority(int UserID, List<CommsMethodPriority> CommsMethod, int CurrentUserID, int CompanyID, CancellationToken token);
    void UserCommsMethods(int UserId, string MethodType, int[] MethodId, int CurrentUserID, int CompanyID, string TimeZoneId);
    Task<bool> UpdateGroupMember(int TargetID, int UserID, int ObjMapID, string Action);
    Task<User> GetRegisteredUserInfo(int CompanyId, int userId);
    Task<bool> UpdateUserMsgGroups(List<UserGroup> UserGroups);
    Task<List<MemberUser>> MembershipList(int ObjMapID, MemberShipType memberShipType, int TargetID, int? Start, int? Length, string? Search, string orderBy, string orderDir, bool ActiveOnly, string? CompanyKey);
    Task<User> DeleteRegisteredUser(int CustomerId, string UniqueGUID, CancellationToken cancellationToken);
    Task<User> UpdateUserPhoto(User User, string UserPhoto, CancellationToken cancellationToken);
    Task<User> UpdateUserPhone(User user, string mobilerNo, string isd, CancellationToken cancellationToken);
    Task<bool> BadEmail(string email);
    Task<bool> DuplicateEmail(string email);
    Task<string> SendInvites(CancellationToken cancellationToken);
    Task<IEnumerable<UserDeviceListModel>> GetAllOneUserDeviceList(int quiredUserId, CancellationToken cancellationToken);
    Task<bool> DeleteUserDevice(int userDeviceId, CancellationToken cancellationToken);
    Task<UserRelations> UserRelations(CancellationToken cancellationToken);
    void ResetUserDeviceToken(int qUserId);
    void CreateUserSecurityGroup(int userId, int securityGroupId, int createdUpatedBy, int companyId, string securityGroupStandatdFilter = "");
    void UserSecurityGroup(int userId, string[] totSecGroup, int currentUserId, int companyId);
    void UserObjectRelation(int userId, string[] objFilters, int currentUserId, int companyId, string timeZoneId, string deptAction = "REPLACE", string locAction = "REPLACE");
    Task<bool> BulkAction(BulkActionModel request, CancellationToken cancellationToken);
    Task<List<UserGroup>> GetUserGroups(int userId);
    Task<dynamic> OffDuty(OffDutyModel request, CancellationToken cancellationToken);
    dynamic OffDutyCheck(OffDuty oncall);
    Task<dynamic> OffDutyStart(OffDutyModel request, CancellationToken cancellationToken);
    Task<List<UserParams>> GetUserSystemInfo(int userId, int companyId);
    Task<dynamic> OffDutyChange(OffDutyModel request, CancellationToken cancellationToken);
    Task<dynamic> OffDutyEnd(OffDutyModel request, bool notifyFront, CancellationToken cancellationToken);
    Task<User> GetUserId(int companyId, string email);
    Task<dynamic> OffDutyActivate(OffDutyModel request, bool exist, CancellationToken cancellationToken);
    Task<dynamic> UpdateGroupMember(int targetId, int userId, int objMapId, string action, int currentUserId, int companyId, string timeZoneId, CancellationToken cancellationToken);
    void CreateOffDutyJob(int userId, DateTimeOffset offDutyDate, int companyId, string action = "START", string timeZoneId = "GMT Standard Time");
    Task<int> CreateUsers(int CompanyId, bool RegisteredUser, string FirstName, string PrimaryEmail, string Password,
        int Status, int CreatedUpdatedBy, string TimeZoneId, string LastName = "", string MobileNo = "", string UserRole = "",
        string UserPhoto = "no-photo.jpg", string ISDCode = "", string LLIsdCode = "", string LandLine = "", string SecondaryEmail = "",
        string UniqueGuiD = "", string Lat = "", string Lng = "", string Token = "", bool ExpirePassword = true, string UserLanguage = "en",
        bool SMSTrigger = false, bool FirstLogin = true, int DepartmentId = 0);
    Task<dynamic> DeleteUser(int userId, int companyId, int currentUserId, string timeZoneId, CancellationToken cancellationToken);
    Task<bool> CheckUserAssociation(int userId, int companyId);
    void UpdateUserComms(int companyId, int userId, int createdUpdatedBy, string timeZoneId = "GMT Standard Time", string pingMethods = "", string incidentMethods = "", bool isNewUser = false, CancellationToken cancellationToken = default);
    void ImportUsercomms(int companyId, string messageType, int userId, List<string> methodList, int createdUpdatedBy, string timeZoneId, string rawMethodsList, CancellationToken cancellationToken);
    Task<UserRelations> UserRelations(int userId, int companyId, string timeZoneId, CancellationToken cancellationToken);
    Task<dynamic> GetUserDashboard(string modulePage, int userId, bool reverse = false);
    Task<dynamic> SaveDashboard(List<DashboardModule> moduleItems, string modulePage, int userId, CancellationToken cancellationToken);
    void AddUserModuleItem(int userId, int moduleId, decimal xPos, decimal yPos, decimal width, decimal height, CancellationToken cancellationToken);
    Task<dynamic> AddDashlet(int moduleId, int userId, decimal xPos, decimal yPos);

}
