using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using FluentValidation;
using MediatR.Extensions.Autofac.DependencyInjection;

namespace CrisesControl.Core
{
    public class MainCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMediatR(ThisAssembly);

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(AbstractValidator<>));

            builder.RegisterAutoMapper(ThisAssembly);
        }
    }
}