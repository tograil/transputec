using System.Configuration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CrisesControl.Core;
using CrisesControl.Infrastructure;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.MongoSettings;
using CrisesControl.Scheduler;
using CrisesControl.Scheduler.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using Quartz.Spi.MongoDbJobStore;
using Serilog;

IConfigurationRoot? conf = null;

var host = Host.CreateDefaultBuilder(args);
host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule(new MainCoreModule());
        containerBuilder.RegisterModule(new MainInfrastructureModule(true));
    });

host.ConfigureAppConfiguration((builderContext, configuration) =>
{
    var env = builderContext.HostingEnvironment;

    configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);


    conf = configuration.Build();
});

host.ConfigureServices(services =>
{
    services.AddHostedService<Worker>();

    services.AddDbContext<CrisesControlContext>(options =>
        options.UseSqlServer(conf.GetConnectionString("CrisesControlDatabase")));

   services.Configure<JobsMongoSettings>(
        conf.GetSection("JobsMongoSettings"));

    services.AddQuartz(q =>
    {
        q.UseMicrosoftDependencyInjectionJobFactory();
    });

    services.AddQuartzHostedService(
        q => q.WaitForJobsToComplete = true);

});

host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration);
});



await host.Build().RunAsync();
