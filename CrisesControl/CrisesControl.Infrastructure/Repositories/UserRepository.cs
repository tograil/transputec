using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.UserAggregate;
using CrisesControl.Core.UserAggregate.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CrisesControl.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CrisesControlContext _context;

    public UserRepository(CrisesControlContext context)
    {
        _context = context;
    }

    public async Task<int> CreateUser(User user, CancellationToken cancellationToken)
    {
        await _context.AddAsync(user, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return user.UserId;
    }

    public bool EmailExists(string email)
    {
        return _context.Set<User>().Any(x => x.PrimaryEmail == email);
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _context.Set<User>().FirstOrDefaultAsync(x => x.UserId == id);
    }

    public async Task UpdateUser(User user, CancellationToken cancellationToken)
    {
        _context.Update(user);

        await _context.SaveChangesAsync(cancellationToken);
    }
}