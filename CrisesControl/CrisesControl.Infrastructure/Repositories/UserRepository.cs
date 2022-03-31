using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UserModel = CrisesControl.Core.Models.EmptyUser;

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

    public async Task<int> UpdateUser(User user, CancellationToken cancellationToken)
    {
        await _context.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user.UserId;
    }

    public int AddPwdChangeHistory(int userId, string newPassword, string timeZoneId)
    {

        var ph = new PasswordChangeHistory
        {
            UserId = userId,
            LastPassword = newPassword,
            ChangedDateTime = DateTime.Now.GetDateTimeOffset(timeZoneId)
        };

        _context.Add(ph);
        _context.SaveChanges();
        return ph.Id;

    }

    public void CreateUserSearch(int userId, string firstName, string lastName, string isdCode, string mobileNo,
        string primaryEmail, int companyId)
    {
        var searchString = firstName + " " + lastName + "|" + primaryEmail + "|" + isdCode + mobileNo;

        var comp = _context.Set<Company>().FirstOrDefault(x => x.CompanyId == companyId);
        if (comp != null)
        {
            var memberUser = _context.Set<MemberUser>().FromSqlRaw("Pro_Create_User_Search {0}, {1}, {2}",
                userId, searchString, comp.UniqueKey!).FirstOrDefault();
        }
    }

    public async Task<User> DeleteUser(User user, CancellationToken token)
    {
        var userToRemove = new User { UserId = user.UserId, CompanyId =  user.CompanyId};
        _context.Remove(userToRemove);
        await _context.SaveChangesAsync(token);
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsers(int companyId)
    {
        return await _context.Set<User>().Where(t => t.CompanyId == companyId).ToListAsync();
    }

    public async Task<User> GetUser(int companyId, int userId)
    {
        return await _context.Set<User>().Where(t => t.CompanyId == companyId && t.UserId == userId).FirstOrDefaultAsync();
    }

    public bool CheckDuplicate(User user)
    {
        return _context.Set<User>().Where(t => t.PrimaryEmail.Equals(user.PrimaryEmail)).Any();
    }
}