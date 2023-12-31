using Autofac.Extensions.DependencyInjection;
using CrisesControl.Auth;
using CrisesControl.Auth.Config;
using CrisesControl.Core.AuditLog.Services;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Context.Misc;
using CrisesControl.Infrastructure.Identity;
using CrisesControl.Infrastructure.Services;
using GrpcAuditLogClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<AuditingInterceptor>();

// Add services to the container.
builder.Services.AddDbContext<CrisesControlContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrisesControlDatabase"));

    options.UseOpenIddict();
});

var serverCredentials = builder.Configuration
                               .GetSection(ServerCredentialsOptions.ServerCredentials)
                               .Get<ServerCredentialsOptions>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
    // configure more options if necessary...
});

builder.Services.AddOpenIddict()

    // Register the OpenIddict core components.
    .AddCore(options => {
        // Configure OpenIddict to use the EF Core stores/models.
        options.UseEntityFrameworkCore()
            .UseDbContext<CrisesControlContext>();
    })

    // Register the OpenIddict server components.
    .AddServer(options => {
        options
            .AllowClientCredentialsFlow()
            .AllowAuthorizationCodeFlow()
            .AllowPasswordFlow();

        options
            .SetTokenEndpointUris("/connect/token")
            .SetAuthorizationEndpointUris("/connect/authorize");

        // Encryption and signing of tokens
        //options
        //    .AddEphemeralEncryptionKey()
        //    .AddEphemeralSigningKey();

        //options.AddEncryptionKey(new SymmetricSecurityKey(
        //    Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        options.AddEncryptionCertificate(serverCredentials.EncryptionKeyThumbprint);
        options.AddSigningCertificate(serverCredentials.SigningKeyThumbprint);

        // Register scopes (permissions)
        options.RegisterScopes("api");

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .DisableTransportSecurityRequirement();

        options.SetAccessTokenLifetime(TimeSpan.FromDays(serverCredentials.TokenExpiryInDays));

    });

builder.Services.AddIdentity<User, IdentityRole>()
    .AddUserStore<UserStore>()
    .AddUserManager<CrisesControlUserManager>();

var auditLogSettings = builder.Configuration.GetSection("AuditLog").Get<AuditLogOptions>();

builder.Services.AddGrpcClient<AuditLogGrpc.AuditLogGrpcClient>(o =>
{
    o.Address = new Uri(auditLogSettings.ServerAddress);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<AuthService>();
builder.Services.AddCors(options => {
    options.AddPolicy("AllowedOrigins",
        builder => {
            builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});

var app = builder.Build();
app.UseCors("AllowedOrigins");
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(setupAction => {
    setupAction.SwaggerEndpoint(builder.Configuration.GetSection("AppName").Value + "/swagger/v1/swagger.json", "CC Core API V1");
});
//if (app.Environment.IsDevelopment()) {
//    app.UseSwagger();
//    app.UseSwaggerUI(c => {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CC Core API V1");
//    });
//} else {
//    app.UseSwagger();
//    app.UseSwaggerUI(c => {
//        c.SwaggerEndpoint("/CriesControlAuth/swagger/v1/swagger.json", "CC Core API V1");
//        c.RoutePrefix = string.Empty;
//    });
//}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



await using (var scope = app.Services.CreateAsyncScope())
{

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

    if (await manager.FindByNameAsync("api") is null)
    {
        await manager.CreateAsync(new OpenIddictScopeDescriptor
        {
            Name = "api",
            Resources =
            {
                "api"
            }
        });
    }
}

app.Run();
