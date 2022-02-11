using CrisesControl.Auth.Config;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace CrisesControl.Auth;

public class AuthService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public AuthService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serverCredentials = _configuration.GetSection(ServerCredentialsOptions.ServerCredentials)
            .Get<ServerCredentialsOptions>();

        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<CrisesControlContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync(serverCredentials.ClientId, cancellationToken) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = serverCredentials.ClientId,
                ClientSecret = serverCredentials.ClientSecret,
                DisplayName = serverCredentials.Profile,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.GrantTypes.Password,

                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                }
            }, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}