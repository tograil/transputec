using Autofac;
using AutoMapper;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using CrisesControl.Api.Application.Behaviours;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core;
using FluentValidation;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Pipeline;

namespace CrisesControl.Api;

public class ApiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterMediatR(ThisAssembly);

        builder.RegisterAssemblyTypes(ThisAssembly)
            .AsClosedTypesOf(typeof(AbstractValidator<>));

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
        builder.RegisterType<SchedulerQuery>().As<ISchedulerQuery>();
        builder.RegisterType<ExTriggerQuery>().As<IExTriggerQuery>();
        builder.RegisterType<CommunicationQuery>().As<ICommunicationQuery>();

        builder.RegisterType<IncidentQuery>().As<IIncidentQuery>();
        builder.RegisterType<TaskQuery>().As<ITaskQuery>();
        builder.RegisterType<AdminQuery>().As<IAdminQuery>();
        builder.RegisterType<RegisterQuery>().As<IRegisterQuery>();
        builder.RegisterType<PaymentQuery>().As<IPaymentQuery>();

        builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();

        builder.RegisterType<CurrentUser>().As<ICurrentUser>();
        builder.RegisterType<AddressQuery>().As<IAddressQuery>();
        builder.RegisterType<SopLibraryQuery>().As<ISopLibraryQuery>();
        builder.RegisterType<ActiveIncidentQuery>().As<IActiveIncidentQuery>();
        builder.RegisterType<SystemQuery>().As<ISystemQuery>();
        builder.RegisterType<AcademyQuery>().As<IAcademyQuery>();
        builder.RegisterType<LookupQuery>().As<ILookupQuery>();
        builder.RegisterType<Mapper>().As<IMapper>();
        builder.RegisterType<SopQuery>().As<ISopQuery>();
        builder.RegisterType<AppQuery>().As<IAppQuery>();
        builder.RegisterType<WebSocketQuery>().As<ICCWebSocketQuery>();

        builder.RegisterAutoMapper(true, typeof(MainCoreModule).Assembly, typeof(ApiModule).Assembly);
    }
}