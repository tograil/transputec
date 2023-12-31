using System.Reflection;
using System.Text;
using Azure.Core;
using Azure.Identity;
using CC.Authority.Api;
using CC.Authority.Api.Config;
using CC.Authority.Implementation;
using CC.Authority.Implementation.Data;
using CC.Authority.Implementation.Identity;
using CC.Authority.Implementation.Models;
using CC.Authority.SCIM.Service.Monitor;
using CrisesControl.Infrastructure.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var monitoringBehavior = new ConsoleMonitor();

builder.Services.AddSingleton(typeof(IMonitor), monitoringBehavior);

builder.Services.AddSpaStaticFiles(configuration => {
    configuration.RootPath = "ClientApp/dist";
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddDbContext<OpenIddictContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrisesControlOpenIddict"));

    options.UseOpenIddict();
});

builder.Services.Configure<IdentityOptions>(options => {
    options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
    // configure more options if necessary...
});

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<OpenIddictContext>();


var serverCredentials = builder.Configuration
    .GetSection(ServerCredentialsOptions.ServerCredentials)
    .Get<ServerCredentialsOptions>();

builder.Services.AddOpenIddict()

    // Register the OpenIddict core components.
    .AddCore(options => {
        // Configure OpenIddict to use the EF Core stores/models.
        options.UseEntityFrameworkCore()
            .UseDbContext<OpenIddictContext>();
    })

    // Register the OpenIddict server components.
    .AddServer(options => {
        options
            .AllowClientCredentialsFlow()
            .AllowAuthorizationCodeFlow()
            .AllowRefreshTokenFlow()
            .AllowPasswordFlow();

        options.UseReferenceAccessTokens().
                UseReferenceRefreshTokens();

        //options.UseDataProtection();

        options.RegisterScopes(OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles,
            OpenIddictConstants.Scopes.OfflineAccess);

        options
            .SetTokenEndpointUris("/connect/token", "/user/token")
            .SetAuthorizationEndpointUris("/connect/authorize")
            .SetIntrospectionEndpointUris("/connect/introspect");

        // Encryption and signing of tokens (for development)
        //options
        //    .AddEphemeralEncryptionKey()
        //    .AddEphemeralSigningKey();

        options.AddEncryptionCertificate(serverCredentials.EncryptionKeyThumbprint);
        options.AddSigningCertificate(serverCredentials.SigningKeyThumbprint);

        // Register scopes (permissions)
        //options.RegisterScopes("api");

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .DisableTransportSecurityRequirement();

        options.SetAccessTokenLifetime(TimeSpan.FromDays(serverCredentials.TokenExpiryInDays));
        options.SetAuthorizationCodeLifetime(TimeSpan.FromDays(serverCredentials.TokenExpiryInDays));
        options.SetRefreshTokenLifetime(TimeSpan.FromDays(serverCredentials.TokenExpiryInDays));

    }).AddValidation(options => {
        // Note: the validation handler uses OpenID Connect discovery
        // to retrieve the address of the introspection endpoint.
        options.SetIssuer(serverCredentials.OpendIddictEndpoint);
        options.EnableTokenEntryValidation();
        options.EnableAuthorizationEntryValidation();

        //options.AddAudiences("api");

        options.AddEncryptionCertificate(serverCredentials.EncryptionKeyThumbprint);

        //options.AddEncryptionKey(new SymmetricSecurityKey(
        //    Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        // Register the System.Net.Http integration.
        options.UseSystemNetHttp();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

builder.Services.AddIdentityCore<User>()
    .AddUserStore<UserStore>()
    .AddUserManager<CrisesControlUserManager>()
    .AddSignInManager<CrisesControlSignInManager>();

/*builder.Services.AddControllers().AddNewtonsoftJson(opt =>
{
    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    opt.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});*/

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Crises Control API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
        Flows = new OpenApiOAuthFlows {
            Password = new OpenApiOAuthFlow {
                Scopes = new Dictionary<string, string> {
                    ["offline_access"] = "api scope description"
                },
                TokenUrl = new Uri(serverCredentials.OpendIddictEndpoint + "connect/token"),
            },
        },
        In = ParameterLocation.Header,
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.OAuth2
    }
    );
    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                            { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
                    },
                    new[] { "offline_access" }
                }
        }
    );
}
);

builder.Services.AddHostedService<AuthService>();
builder.Services.AddCors(options => {
    options.AddPolicy("AllowedOrigins",
        builder => {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

bool isAzureDb = Convert.ToBoolean(builder.Configuration["IsAzureDataBase"]);

if (isAzureDb) {
    TokenCredential credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions {
        ManagedIdentityClientId = builder.Configuration["AzureADManagedIdentityClientId"]
    });

    if (builder.Environment.IsProduction()) {
        credentials = new ManagedIdentityCredential();
    }

    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        credentials);

    var akvProvider = new SqlColumnEncryptionAzureKeyVaultProvider(credentials);

    SqlConnection.RegisterColumnEncryptionKeyStoreProviders(customProviders: new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>(capacity: 1, comparer: StringComparer.OrdinalIgnoreCase)
    {
    { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, akvProvider}

    });
}


builder.Services.AddDbContext<CrisesControlAuthContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrisesControlDatabase"));
});
void ConfigureMvcNewtonsoftJsonOptions(MvcNewtonsoftJsonOptions options) => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

//builder.Services.AddAuthentication(ConfigureAuthenticationOptions).AddJwtBearer(ConfigureJwtBearerOptons);
builder.Services.AddControllers().AddNewtonsoftJson(ConfigureMvcNewtonsoftJsonOptions);

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

//builder.Services.AddAuthentication(cfg =>
//{
//    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//       .AddJwtBearer(cfg => {
//           cfg.RequireHttpsMetadata = false;
//           cfg.SaveToken = true;
//       });


builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Asif") {
    app.UseSwagger();
    app.UseSwaggerUI(setupAction => {
        setupAction.OAuthClientId(serverCredentials.ClientId);
        setupAction.OAuthClientSecret(serverCredentials.ClientSecret);
    });
}

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseStaticFiles();

if (!app.Environment.IsDevelopment()) {
    app.UseSpaStaticFiles();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

await using (var scope = app.Services.CreateAsyncScope()) {
    var context = scope.ServiceProvider.GetService<OpenIddictContext>();
    context.Database.Migrate();

    var context2 = scope.ServiceProvider.GetService<CrisesControlAuthContext>();
    context2.Database.Migrate();

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
    var existingClientApp = manager.FindByClientIdAsync(serverCredentials.ClientId).GetAwaiter().GetResult();
    if (existingClientApp == null) {
        manager.CreateAsync(new OpenIddictApplicationDescriptor {
            ClientId = serverCredentials.ClientId,
            ClientSecret = serverCredentials.ClientSecret,
            DisplayName = "Default client application",
            Permissions = {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles
            }
        }).GetAwaiter().GetResult();
    }

    //var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

    //if (await manager.FindByNameAsync("api") is null) {
    //    await manager.CreateAsync(new OpenIddictScopeDescriptor {
    //        Name = "api",
    //        Resources =
    //        {
    //            "api"
    //        }
    //    });
    //}
}

if (app.Environment.IsDevelopment()) {
    app.UseSpa(spa => {
        // To learn more about options for serving an Angular SPA from ASP.NET Core,
        // see https://go.microsoft.com/fwlink/?linkid=864501

        spa.Options.SourcePath = "ClientApp";

        if (app.Environment.IsDevelopment()) {
            spa.UseAngularCliServer(npmScript: "start");
            // spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
        }
    });
}

app.Run();