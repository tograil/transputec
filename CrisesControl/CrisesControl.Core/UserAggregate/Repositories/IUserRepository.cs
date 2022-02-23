using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.UserAggregate.Repositories;

public interface IUserRepository
{
    Task<int> CreateUser(User user, CancellationToken cancellationToken);

    bool EmailExists(string email);

    Task<User?> GetUserById(int id);

    Task UpdateUser(User user, CancellationToken cancellationToken);
}