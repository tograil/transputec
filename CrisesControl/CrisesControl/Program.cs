using Autofac;
using Autofac.Extensions.DependencyInjection;
using CrisesControl.Api;
using CrisesControl.Config;
using CrisesControl.Core;
using CrisesControl.Infrastructure;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.MongoSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Serilog;
using System.Reflection;
using CrisesControl.Api.Maintenance;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.AuditLog.Services;
using CrisesControl.Infrastructure.Context.Misc;
using CrisesControl.Infrastructure.Services;
using GrpcAuditLogClient;
using System.Net.WebSockets;
using CrisesControl.Infrastructure.Repositories;
using CrisesControl.Core.CCWebSocket.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.WebHost.UseUrls("http://localhost:7010");

//Register DI which not working with autofac
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<AuditingInterceptor>();

builder.Services.AddDbContext<CrisesControlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrisesControlDatabase")), ServiceLifetime.Transient);

builder.Services.Configure<JobsMongoSettings>(
    builder.Configuration.GetSection("JobsMongoSettings"));
builder.Services.Configure<AuditLogOptions>(builder.Configuration.GetSection("AuditLog"));

// Add services to the container.

builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

var serverCredentials = builder.Configuration
                               .GetSection(ServerCredentialsOptions.ServerCredentials)
                               .Get<ServerCredentialsOptions>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.OperationFilter<SwaggerParameterAttributeFilter>();
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

var auditLogSettings = builder.Configuration.GetSection("AuditLog").Get<AuditLogOptions>();

builder.Services.AddGrpcClient<AuditLogGrpc.AuditLogGrpcClient>(o =>
{
    o.Address = new Uri(auditLogSettings.ServerAddress);
});

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration);
});

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => {
    containerBuilder.RegisterModule(new ApiModule());
    containerBuilder.RegisterModule(new MainCoreModule());
    containerBuilder.RegisterModule(new MainInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});

builder.Services.AddOpenIddict()
    .AddValidation(options => {
        // Note: the validation handler uses OpenID Connect discovery
        // to retrieve the address of the introspection endpoint.
        options.SetIssuer(serverCredentials.OpendIddictEndpoint);
        options.AddAudiences("api");

        options.AddEncryptionCertificate(serverCredentials.EncryptionKeyThumbprint);

        //options.AddEncryptionKey(new SymmetricSecurityKey(
        //    Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        // Register the System.Net.Http integration.
        options.UseSystemNetHttp();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

//builder.Services.AddControllers(o =>
// {
//     var policy = new AuthorizationPolicyBuilder()
//                .RequireAuthenticatedUser()
//                .Build();

//     o.Filters.Add(new AuthorizeFilter(policy));
// }
//);
builder.Services.AddScoped<IPaging, Paging>();

builder.Services.AddControllers(o =>
{
    o.Filters.Add<PagedGetResourceFilter>();
    //o.Filters.Add<PagedGetResultFilter>();
    o.Filters.Add<ErrorFilter>();
});

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(setupAction => {
    setupAction.OAuthClientId(serverCredentials.ClientId);
    setupAction.OAuthClientSecret(serverCredentials.ClientSecret);
    setupAction.SwaggerEndpoint(builder.Configuration.GetSection("AppName")
               .Value + "/swagger/v1/swagger.json", "CC Core API V1");
});
var wsOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) };
app.UseWebSockets(wsOptions);
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            try
            {
                var handler = app.Services.GetRequiredService<ICCWebSocketRepository>();
                await handler.ProcessWebsocketSession(context, webSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
        }
    }
    else
    {
        await next();
    }
});
//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
//    app.UseSwagger();
//    app.UseSwaggerUI(setupAction => {
//        setupAction.OAuthClientId(serverCredentials.ClientId);
//        setupAction.OAuthClientSecret(serverCredentials.ClientSecret);
//        setupAction.SwaggerEndpoint(builder.Configuration.GetSection("AppName").Value + "/swagger/v1/swagger.json", "CC Core API V1");
//    });
//} else {
//    app.UseSwagger();
//    app.UseSwaggerUI(setupAction => {
//        setupAction.OAuthClientId(serverCredentials.ClientId);
//        setupAction.OAuthClientSecret(serverCredentials.ClientSecret);
//        setupAction.SwaggerEndpoint(builder.Configuration.GetSection("AppName").Value + "/swagger/v1/swagger.json", "CC Core API V1");
//    });
//}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
