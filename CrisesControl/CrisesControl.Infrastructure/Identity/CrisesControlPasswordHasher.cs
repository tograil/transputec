using CrisesControl.Core.Models;
using CrisesControl.Core.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace CrisesControl.Infrastructure.Identity;

public class CrisesControlPasswordHasher : IPasswordHasher<User>
{
    public string HashPassword(User user, string password)
    {
        return password;
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        return hashedPassword == providedPassword ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
}