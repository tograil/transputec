using Autofac;
using CrisesControl.Core.CompanyAggregate.Handlers.GetCompany;
using FluentValidation;
using MediatR.Extensions.Autofac.DependencyInjection;

namespace CrisesControl.Core
{
    public class MainCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMediatR(typeof(MainCoreModule).Assembly);

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(AbstractValidator<>));
        }
    }
}