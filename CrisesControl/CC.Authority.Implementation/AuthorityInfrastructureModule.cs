using System.Reflection;
using CC.Authority.Core.UserManagement;
using CC.Authority.Implementation.Config;
using CC.Authority.Implementation.Helpers;
using CC.Authority.Implementation.Scim;
using CC.Authority.Implementation.Services;
using CC.Authority.SCIM.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CC.Authority.Implementation
{
    public static class AuthorityInfrastructureModule
    {
        public static void AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddScoped<IProvider, ScimProvider>();

            services.AddAutoMapper(Assembly.GetAssembly(typeof(AuthorityInfrastructureModule)));

            services.AddScoped<ICurrentUser, CurrentUser>();

            services.Configure<CrisesControlServerConfig>(configuration.GetSection(CrisesControlServerConfig.Name));

            services.AddHttpClient<IUserManager, UserManager>();
        }
    }
}