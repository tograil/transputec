using CC.Authority.Implementation.Scim;
using CC.Authority.SCIM.Service;
using Microsoft.Extensions.DependencyInjection;

namespace CC.Authority.Implementation
{
    public static class AuthorityInfrastructureModule
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IProvider, ScimProvider>();
        }
    }
}