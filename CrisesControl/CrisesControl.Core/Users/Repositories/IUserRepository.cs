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

    void CreateUserSearch(int userId, string firstName, string lastName, string isdCode, string mobileNo,
        string primaryEmail, int companyId);
    Task<IEnumerable<User>> GetAllUsers(int companyId);
    Task<User> GetUser(int companyId, int userId);
    Task<User> DeleteUser(User user, CancellationToken token);
    bool CheckDuplicate(User user);
    LoginInfoReturnModel GetLoggedInUserInfo(LoginInfo request, CancellationToken cancellationToken);

}