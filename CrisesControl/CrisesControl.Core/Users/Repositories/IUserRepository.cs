using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
    Task<IEnumerable<User>> GetAllUsers(GetAllUserRequest userRequest);
    Task<User> GetUser(int companyId, int userId);
    Task<User> DeleteUser(User user, CancellationToken token);
    bool CheckDuplicate(User user);
    Task<LoginInfoReturnModel> GetLoggedInUserInfo(LoginInfo request, CancellationToken cancellationToken);
    Task<User> ReactivateUser(int qureiedUserId, CancellationToken cancellationToken);

    Task<int> UpdateProfile(User user);
    Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "");
    void CreateSMSTriggerRight(int CompanyId, int UserId, string UserRole, bool SMSTrigger, string ISDCode, string MobileNo, bool Self = false);
    void UserCommsPriority(int UserID, List<CommsMethodPriority> CommsMethod, int CurrentUserID, int CompanyID, CancellationToken token);
    void UserCommsMethods(int UserId, string MethodType, int[] MethodId, int CurrentUserID, int CompanyID, string TimeZoneId);

    Task<User> GetRegisteredUserInfo(int CompanyId, int userId);
    Task<bool> UpdateUserMsgGroups(List<UserGroup> UserGroups);
}