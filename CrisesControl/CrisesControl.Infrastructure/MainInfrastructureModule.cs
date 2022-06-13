using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using CrisesControl.Core.Groups.Repositories;
using CrisesControl.Core.Locations.Services;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Repositories;
using CrisesControl.Core.Assets.Respositories;
using CrisesControl.Core.Incidents.Services;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Core.Queues.Repositories;
using CrisesControl.Core.Queues.Services;
using CrisesControl.Core.Settings.Repositories;
using CrisesControl.Infrastructure.Services;
using CrisesControl.Core.Billing.Repositories;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Core.Communication.Repositories;
using CrisesControl.Core.Security;
using CrisesControl.Core.CompanyParameters.Repositories;
using Microsoft.Extensions.Caching.Memory;
using CrisesControl.Core.Jobs.Repositories;
using CrisesControl.Core.ExTriggers.Repositories;
using CrisesControl.Core.Jobs.Services;
using CrisesControl.Core.Tasks.Repositories;
using CrisesControl.Core.Register.Repositories;
using CrisesControl.Core.Administrator.Repositories;

namespace CrisesControl.Infrastructure
{
    public class MainInfrastructureModule : Module
    {
        private static bool _isDevelopment;

        public MainInfrastructureModule(bool isDevelopment)
        {
            _isDevelopment = isDevelopment;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_isDevelopment)
            {
                RegisterDevelopmentOnlyDependencies(builder);
            }
            else
            {
                RegisterProductionOnlyDependencies(builder);
            }

            RegisterCommonDependencies(builder);
        }

        private void RegisterCommonDependencies(ContainerBuilder builder)
        {
            builder.RegisterAutoMapper(ThisAssembly);

            builder.RegisterType<CompanyRepository>().As<ICompanyRepository>();
            builder.RegisterType<RegisterCompanyRepository>().As<IRegisterCompanyRepository>();
            builder.RegisterType<DepartmentRepository>().As<IDepartmentRepository>();
            builder.RegisterType<GroupRepository>().As<IGroupRepository>();
            builder.RegisterType<LocationRepository>().As<ILocationRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<GlobalParametersRepository>().As<IGlobalParametersRepository>();
            builder.RegisterType<DepartmentRepository>().As<IDepartmentRepository>();
            builder.RegisterType<IncidentRepository>().As<IIncidentRepository>();
            builder.RegisterType<TaskRepository>().As<ITaskRepository>();
            builder.RegisterType<ActiveIncidentRepository>().As<IActiveIncidentRepository>();
            builder.RegisterType<MessageRepository>().As<IMessageRepository>();
            builder.RegisterType<AssetRespository>().As<IAssetRepository>();
            builder.RegisterType<JobRepository>().As<IJobRepository>();
            builder.RegisterType<JobScheduleRepository>().As<IJobScheduleRepository>();
            builder.RegisterType<SettingsRepository>().As<ISettingsRepository>().SingleInstance();
            builder.RegisterType<QueueRepository>().As<IQueueRepository>();


            builder.RegisterType<MessageService>().As<IMessageService>();
            builder.RegisterType<ActiveIncidentTaskService>().As<IActiveIncidentTaskService>();
            builder.RegisterType<ReportsRepository>().As<IReportsRepository>();
            builder.RegisterType<CommunicationRepository>().As<ICommunicationRepository>();
            builder.RegisterType<SecurityRepository>().As<ISecurityRepository>();
            builder.RegisterType<CompanyParametersRepository>().As<ICompanyParametersRepository>();
            builder.RegisterType<ScheduleService>().As<IScheduleService>();
            builder.RegisterType<QueueService>().As<IQueueService>();
            builder.RegisterType<QueueMessageService>().As<IQueueMessageService>();
            builder.RegisterType<IncidentService>().As<IIncidentService>();
            builder.RegisterType<CommunicationRepository>().As<ICommunicationRepository>();
            builder.RegisterType<ExTriggerRepository>().As<IExTriggerRepository>();
            builder.RegisterType<ScheduleService>().As<IScheduleService>();
            builder.RegisterType<RegisterRepository>().As<IRegisterRepository>();
            builder.RegisterType<AdminRepository>().As<IAdminRepository>();
            builder.RegisterType<SendEmailService>().As<ISenderEmailService>();
        }

        private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
        {
            // TODO: Add development only services
        }

        private void RegisterProductionOnlyDependencies(ContainerBuilder builder)
        {
            // TODO: Add production only services
        }
    }
}