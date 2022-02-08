using Autofac;
using Autofac.Extensions.DependencyInjection;
using CrisesControl.Core;
using CrisesControl.Infrastructure;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddDbContext<CrisesControlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrisesControlDatabase")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new MainCoreModule());
    containerBuilder.RegisterModule(new MainInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
