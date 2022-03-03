using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Departments.Repositories;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Repositories;

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
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<GlobalParametersRepository>().As<IGlobalParametersRepository>();
            builder.RegisterType<DepartmentRepository>().As<IDepartmentRepository>();
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