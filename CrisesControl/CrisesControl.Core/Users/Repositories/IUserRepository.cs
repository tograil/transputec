using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users.Repositories;

public interface IUserRepository
{
    Task<int> CreateUser(User user, CancellationToken cancellationToken);
    bool EmailExists(string email);
    Task<User?> GetUserById(int id);
    Task UpdateUser(User user, CancellationToken cancellationToken);
    int AddPwdChangeHistory(int userId, string newPassword, string timeZoneId);

    void CreateUserSearch(int userId, string firstName, string lastName, string isdCode, string mobileNo,
        string primaryEmail, int companyId);
}