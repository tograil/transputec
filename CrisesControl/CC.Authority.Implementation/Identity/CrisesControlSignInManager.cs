using CC.Authority.Implementation.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Authority.Implementation.Identity {
    public class CrisesControlSignInManager : SignInManager<User> {
        public CrisesControlSignInManager(UserManager<User> userManager, 
            IHttpContextAccessor contextAccessor, 
            IUserClaimsPrincipalFactory<User> claimsFactory, 
            IOptions<IdentityOptions> optionsAccessor, 
            ILogger<SignInManager<User>> logger, 
            IAuthenticationSchemeProvider schemes, 
            IUserConfirmation<User> confirmation) : 
            base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation) {
        }
    }
}
