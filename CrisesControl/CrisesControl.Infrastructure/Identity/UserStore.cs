using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Models;
using CrisesControl.Core.UserAggregate;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CrisesControl.Infrastructure.Identity;

public class UserStore : IUserPasswordStore<User>, IUserRoleStore<User>
{
    private readonly CrisesControlContext _context;

    public UserStore(CrisesControlContext context)
    {
        _context = context;
    }

    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserId.ToString());
    }

    public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PrimaryEmail);
    }

    public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PrimaryEmail.ToUpperInvariant());
    }

    public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        var result = await _context.AddAsync(user, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return result.State == EntityState.Added ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _context.Update(user);

        await _context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken) 
        => await _context.Set<User>().FirstAsync(x => x.UserId.ToString().Equals(userId), cancellationToken);

    public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var stringToCompare = normalizedUserName.ToLowerInvariant();

        return await _context.Set<User>()
            .FirstOrDefaultAsync(x => x.PrimaryEmail.Equals(stringToCompare),
                cancellationToken);
    }

    public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Password);

    public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken) 
        => Task.FromResult(!string.IsNullOrWhiteSpace(user.Password));

    public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
    }
}