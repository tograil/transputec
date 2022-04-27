using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using CrisesControl.Core.GroupAggregate.Repositories;
using CrisesControl.Core.LocationAggregate.Services;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Repositories;
using CrisesControl.Core.AssetAggregate.Respositories;
using CrisesControl.Core.Incidents.Services;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Infrastructure.Services;
using CrisesControl.Core.Billing.Repositories;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Communication.Repositories;

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
            builder.RegisterType<ActiveIncidentRepository>().As<IActiveIncidentRepository>();
            builder.RegisterType<MessageRepository>().As<IMessageRepository>();
            builder.RegisterType<AssetRespository>().As<IAssetRepository>();
            builder.RegisterType<BillingRespository>().As<IBillingRepository>();
            builder.RegisterType<MessageService>().As<IMessageService>();
            builder.RegisterType<ActiveIncidentTaskService>().As<IActiveIncidentTaskService>();
            builder.RegisterType<ReportRepository>().As<IReportsRepository>();
            builder.RegisterType<CompanyParametersRepository>().As<ICompanyParametersRepository>();
            builder.RegisterType<CommunicationRepository>().As<ICommunicationRepository>();
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