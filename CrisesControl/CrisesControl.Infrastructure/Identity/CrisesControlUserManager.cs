using System;
using System.Collections.Generic;
using CrisesControl.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CrisesControl.Infrastructure.Identity;

public class CrisesControlUserManager : UserManager<User>
{
    public CrisesControlUserManager(IUserStore<User> store,
        IOptions<IdentityOptions> optionsAccessor,
        IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<User>> logger) 
        : base(store,
            optionsAccessor, new CrisesControlPasswordHasher(), userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }
}