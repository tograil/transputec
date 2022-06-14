using System.Reflection;
using Azure.Core;
using Azure.Identity;
using CC.Authority.Api;
using CC.Authority.Api.Config;
using CC.Authority.Implementation;
using CC.Authority.Implementation.Data;
using CC.Authority.Implementation.Models;
using CC.Authority.Implementation.Scim;
using CC.Authority.SCIM.Service;
using CC.Authority.SCIM.Service.Monitor;
using CrisesControl.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Microsoft.SCIM.WebHostSample.Provider;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var monitoringBehavior = new ConsoleMonitor();

builder.Services.AddSingleton(typeof(IMonitor), monitoringBehavior);

builder.Services.AddInfrastructure();

builder.Services.AddDbContext<OpenIddictContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrisesControlOpenIddict"));

    options.UseOpenIddict();
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
    // configure more options if necessary...
});

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
            .AllowPasswordFlow();

        options
            .SetTokenEndpointUris("/connect/token")
            .SetAuthorizationEndpointUris("/connect/authorize");

        // Encryption and signing of tokens
        options
            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey();

        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        // Register scopes (permissions)
        options.RegisterScopes("api");

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .DisableTransportSecurityRequirement();

    }).AddValidation(options => {
        // Note: the validation handler uses OpenID Connect discovery
        // to retrieve the address of the introspection endpoint.
        options.SetIssuer(serverCredentials.OpendIddictEndpoint);
        options.AddAudiences("api");


        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        // Register the System.Net.Http integration.
        options.UseSystemNetHttp();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

builder.Services.AddIdentityCore<User>()
    .AddUserStore<UserStore>()
    .AddUserManager<CrisesControlUserManager>();

builder.Services.AddControllers().AddNewtonsoftJson(opt =>
{
    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    opt.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Crises Control API", Version = "v1" });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        Scopes = new Dictionary<string, string>
                        {
                            ["api"] = "api scope description"
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
                    new[] { "api" }
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

builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        ManagedIdentityClientId = builder.Configuration["AzureADManagedIdentityClientId"]
    }));

var credentials = new VisualStudioCredential();

var akvProvider = new SqlColumnEncryptionAzureKeyVaultProvider(credentials);

SqlConnection.RegisterColumnEncryptionKeyStoreProviders(customProviders: new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>(capacity: 1, comparer: StringComparer.OrdinalIgnoreCase)
{
    { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, akvProvider}
});

builder.Services.AddDbContext<CrisesControlAuthContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrisesControlDatabase"));
});

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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