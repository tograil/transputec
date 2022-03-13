using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR.Extensions.Autofac.DependencyInjection;

namespace CrisesControl.Api;

public class ApiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterMediatR(ThisAssembly);

        builder.RegisterAssemblyTypes(ThisAssembly)
            .AsClosedTypesOf(typeof(AbstractValidator<>));

        builder.RegisterAutoMapper(ThisAssembly);

        builder.RegisterType<CompanyQuery>().As<ICompanyQuery>();

        builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();

        builder.RegisterType<CurrentUser>().As<ICurrentUser>();
    }
}