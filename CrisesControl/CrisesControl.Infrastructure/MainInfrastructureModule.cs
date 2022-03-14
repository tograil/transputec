﻿using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using CrisesControl.Core.CompanyAggregate.Repositories;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using CrisesControl.Core.GroupAggregate.Repositories;
using CrisesControl.Core.LocationAggregate.Services;
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
            builder.RegisterType<DepartmentRepository>().As<IDepartmentRepository>();
            builder.RegisterType<GroupRepository>().As<IGroupRepository>();
            builder.RegisterType<LocationRepository>().As<ILocationRepository>();
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