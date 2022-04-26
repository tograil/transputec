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
        builder.RegisterType<BillingQuery>().As<IBillingQuery>();
        builder.RegisterType<DepartmentQuery>().As<IDepartmentQuery>();
        builder.RegisterType<GroupQuery>().As<IGroupQuery>();
        builder.RegisterType<LocationQuery>().As<ILocationQuery>();
        builder.RegisterType<AssetQuery>().As<IAssetQuery>();
        builder.RegisterType<UserQuery>().As<IUserQuery>();
        builder.RegisterType<MessageQuery>().As<IMessageQuery>();
        builder.RegisterType<ReportsQuery>().As<IReportsQuery>();
        builder.RegisterType<CommunicationQuery>().As<ICommunicationQuery>();
        builder.RegisterType<SecurityQuery>().As<ISecurityQuery>();
        builder.RegisterType<CompanyParametersQuery>().As<ICompanyParametersQuery>();


        builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();

        builder.RegisterType<CurrentUser>().As<ICurrentUser>();
    }
}