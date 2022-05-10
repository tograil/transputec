using Autofac;
using Autofac.Extensions.DependencyInjection;
using CrisesControl.Api;
using CrisesControl.Config;
using CrisesControl.Core;
using CrisesControl.Infrastructure;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddDbContext<CrisesControlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrisesControlDatabase")));

// Add services to the container.

builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

var serverCredentials = builder.Configuration
                               .GetSection(ServerCredentialsOptions.ServerCredentials)
                               .Get<ServerCredentialsOptions>();
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


        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

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
builder.Services.AddControllers();

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
