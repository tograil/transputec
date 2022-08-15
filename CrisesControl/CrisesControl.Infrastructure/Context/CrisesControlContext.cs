using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Communication;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Queues;
using CrisesControl.Core.Users;
using CrisesControl.Core.Incidents.SPResponse;
using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using CrisesControl.Core.Security;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Tasks.SP_Response;
using CrisesControl.Core.Common;

using CrisesControl.Core.ExTriggers;
using CrisesControl.Core.Jobs;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Infrastructure.Context.Misc;
using CrisesControl.Core.Reports.SP_Response;
using CrisesControl.Core.Groups;
using CrisesControl.Core.Register;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.System;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Academy;


namespace CrisesControl.Infrastructure.Context
{
    public class CrisesControlContext : DbContext
    {
        private readonly AuditingInterceptor _auditingInterceptor;

        public CrisesControlContext(AuditingInterceptor auditingInterceptor)
        {
            _auditingInterceptor = auditingInterceptor;
        }

        public CrisesControlContext(DbContextOptions<CrisesControlContext> options,
            AuditingInterceptor auditingInterceptor)
            : base(options)
        {
            _auditingInterceptor = auditingInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditingInterceptor);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_CI_AS");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrisesControlContext).Assembly);

            modelBuilder.Entity<NewIncident>().HasNoKey();
            modelBuilder.Entity<MessageQueueItem>().HasNoKey();
            modelBuilder.Entity<CompanySubscribedMethod>().HasNoKey();
            modelBuilder.Entity<UserMessageList>().HasNoKey().Ignore("SentBy");
            modelBuilder.Entity<UserFullName>().HasNoKey();
            modelBuilder.Entity<PhoneNumber>().HasNoKey();
            modelBuilder.Entity<SOSItem>().HasNoKey();
            modelBuilder.Entity<ConferenceDetails>().HasNoKey();
            modelBuilder.Entity<CompanySecurityGroup>().HasNoKey();
            modelBuilder.Entity<CommsMethodPriority>().HasNoKey();
            modelBuilder.Entity<CascadingPlanReturn>().HasNoKey();
            modelBuilder.Entity<JobList>().HasNoKey();
            modelBuilder.Entity<ExTriggerList>().HasNoKey();
            modelBuilder.Entity<MessageAcknowledgements>().HasNoKey().Ignore("AcknowledgedUser").Ignore("UserMobile").Ignore("UserLandLine"); 
            modelBuilder.Entity<Order>().HasNoKey();
            modelBuilder.Entity<Search>().HasNoKey();
            modelBuilder.Entity<ResponseSummary>().HasNoKey();
            modelBuilder.Entity<MessageReporting>().HasNoKey();
            modelBuilder.Entity<IncidentMessageDetails>().HasNoKey();
            modelBuilder.Entity<MemberUser>().HasNoKey().Ignore("UserFullName");
            modelBuilder.Entity<CompanyParameterItem>().HasNoKey();
            modelBuilder.Entity<UserConferenceItem>().HasNoKey();
            modelBuilder.Entity<IncidentPingStatsCount>().HasNoKey();
            modelBuilder.Entity<MessageAcknowledgements>().HasNoKey();
            modelBuilder.Entity<DataTablePaging>().HasNoKey().Ignore("data");
            modelBuilder.Entity<AcknowledgeReturn>().HasNoKey();
            modelBuilder.Entity<IncidentList>().HasNoKey();
            modelBuilder.Entity<AffectedLocation>().HasNoKey();
            modelBuilder.Entity<UpdateIncidentStatusReturn>().HasNoKey();
            modelBuilder.Entity<IncidentDetails>().HasNoKey();
            modelBuilder.Entity<CommsMethods>().HasNoKey();
            modelBuilder.Entity<PingGroupChartCount>().HasNoKey();
            modelBuilder.Entity<DeliveryOutput>().HasNoKey();
            modelBuilder.Entity<TrackUserCount>().HasNoKey();

            modelBuilder.Entity<AckOption>().HasNoKey();
            modelBuilder.Entity<IncKeyCons>().HasNoKey();
            modelBuilder.Entity<IncidentAssetResponse>().HasNoKey();
            modelBuilder.Entity<ActionLsts>().HasNoKey();
            modelBuilder.Entity<IncidentParticipants>().HasNoKey();
            modelBuilder.Entity<UserFullName>().HasNoKey();
            modelBuilder.Entity<Predecessor>().HasNoKey();
            modelBuilder.Entity<TaskGroup>().HasNoKey();
            modelBuilder.Entity<TaskUser>().HasNoKey();
            modelBuilder.Entity<GetCheckListReponseOption>().HasNoKey();
            modelBuilder.Entity<CheckListUpsert>().HasNoKey();
            modelBuilder.Entity<JsonResult>().HasNoKey();
            modelBuilder.Entity<CurrentIncidentStatsResponse>().HasNoKey();
            modelBuilder.Entity<IncidentData>().HasNoKey().Ignore("KeyContacts").Ignore("IncidentAssets");
            modelBuilder.Entity<IncidentDataByActivationRefKeyContactsResponse>().HasNoKey();
            modelBuilder.Entity<IncidentDataByActivationRefIncidentAssetsResponse>().HasNoKey();
            modelBuilder.Entity<IncidentMessageAuditResponse>().HasNoKey().Ignore("SentBy");
            modelBuilder.Entity<IncidentUserLocationResponse>().HasNoKey().Ignore("UserName").Ignore("UserMobile");
            modelBuilder.Entity<TrackUsers>().HasNoKey();
            modelBuilder.Entity<MessageDetails>().HasNoKey();
            //modelBuilder.Entity<Assets>().HasNoKey();
            modelBuilder.Entity<MessageGroupObject>().HasNoKey();
            modelBuilder.Entity<GroupLink>().HasNoKey();

            modelBuilder.Entity<JsonResults>().HasNoKey();
            modelBuilder.Entity<DeliverySummary>().HasNoKey();


            modelBuilder.Entity<LibIncident>().HasNoKey();
            modelBuilder.Entity<CommsStatus>().HasNoKey();
            modelBuilder.Entity<CompanyUser>().HasNoKey().Ignore("UserName");
            modelBuilder.Entity<GetCompanyDataResponse>().HasNoKey();




            //modelBuilder.Entity<Location>().HasNoKey();
            modelBuilder.Entity<Result>().HasNoKey();
            modelBuilder.Entity<MessageGroupObject>().HasNoKey();
            modelBuilder.Entity<PriorityMethod>().HasNoKey();
            modelBuilder.Entity<ReportParam>().HasNoKey();
            modelBuilder.Entity<AdminLibIncident>().HasNoKey();
            modelBuilder.Entity<AdminResult>().HasNoKey();
            modelBuilder.Entity<CompanyPackageFeatureList>().HasNoKey();
            modelBuilder.Entity<TransactionList>().HasNoKey();
            modelBuilder.Entity<CompanyPackageFeatureList>().HasNoKey();
            modelBuilder.Entity<TransactionDtls>().HasNoKey();
            modelBuilder.Entity<UnpaidTransaction>().HasNoKey();
            modelBuilder.Entity<UpdateTransactionDetailsModel>().HasNoKey();
            modelBuilder.Entity<EmailTemplateList>().HasNoKey();
            modelBuilder.Entity<AppLanguages>().HasNoKey();
            modelBuilder.Entity<CompanyPackageItems>().HasNoKey();
            
            //modelBuilder.Entity<SecurityAllObjects>().HasNoKey();
            modelBuilder.Entity<UserTaskHead>().HasNoKey();
            modelBuilder.Entity<IncidentTaskDetails>().HasNoKey();
            modelBuilder.Entity<ActiveTaskParticiants>().HasNoKey();
            modelBuilder.Entity<DeclinedList>().HasNoKey();
            modelBuilder.Entity<ReallocatedList>().HasNoKey();
            modelBuilder.Entity<DelegatedList>().HasNoKey();
            modelBuilder.Entity<TaskIncidentHeader>().HasNoKey();
            modelBuilder.Entity<TwilioPriceList>().HasNoKey();
            modelBuilder.Entity<MessageISDList>().HasNoKey();
            modelBuilder.Entity<TrackMeUsers>().HasNoKey();
            modelBuilder.Entity<TaskAssignedUser>().HasNoKey();
            modelBuilder.Entity<UsrResponse>().HasNoKey();
            modelBuilder.Entity<ActiveCheckList>().HasNoKey();
            modelBuilder.Entity<TaskAudit>().HasNoKey();
            modelBuilder.Entity<FailedTaskList>().HasNoKey();
            modelBuilder.Entity<IncidentMessagesRtn>().HasNoKey();
            modelBuilder.Entity<Incidents>().HasNoKey();
            modelBuilder.Entity<CallToAction>().HasNoKey();
            modelBuilder.Entity<IncidentSOSRequest>().HasNoKey();
            modelBuilder.Entity<IncidentTask>().HasNoKey();
            modelBuilder.Entity<CrisesControl.Core.Reports.TrackingExport>().HasNoKey();
            modelBuilder.Entity<AcademyVideos>().HasNoKey();
            
           // modelBuilder.Entity<SecurityAllObjects>().HasNoKey();
            modelBuilder.Entity<CompanyCommunication>().HasNoKey();
            modelBuilder.Entity<ReplyChannel>().HasNoKey();
            modelBuilder.Entity<CompanyAccount>().HasNoKey();



            //modelBuilder.Entity<CrisesControl.Core.Incidents.IncidentMessagesRtn>().HasNoKey();
            modelBuilder.Entity<UserPieChart>().HasNoKey();
            modelBuilder.Entity<UserIncidentReportResponse>().HasNoKey();
            modelBuilder.Entity<IncidentUserMessageResponse>().HasNoKey();
            modelBuilder.Entity<IncidentStatsResponse>().HasNoKey();
            modelBuilder.Entity<IncidentStat>().HasNoKey();
            modelBuilder.Entity<PerformanceReport>().HasNoKey();
            modelBuilder.Entity<PingReportGrid>().HasNoKey();
            modelBuilder.Entity<ResponseCordinates>().HasNoKey();
            //modelBuilder.Entity<TrackingExport>().HasNoKey();
            modelBuilder.Entity<TaskPerformance>().HasNoKey();
            modelBuilder.Entity<FailedTaskReport>().HasNoKey();
            //modelBuilder.Entity<FailedTaskList>().HasNoKey();
            modelBuilder.Entity<UserItems>().HasNoKey();
            modelBuilder.Entity<IncidentResponseSummary>().HasNoKey();
            modelBuilder.Entity<FailedAttempts>().HasNoKey();
            modelBuilder.Entity<DeliveryDetails>().HasNoKey();
            modelBuilder.Entity<PingReport>().HasNoKey();
            modelBuilder.Entity<CategoryTag>().HasNoKey();
            modelBuilder.Entity<AdminTransaction>().HasNoKey();
            modelBuilder.Entity<PackageAddons>().HasNoKey();
            modelBuilder.Entity<Sectors>().HasNoKey();
            modelBuilder.Entity<CrisesControl.Core.Administrator.Api>().HasNoKey();
            modelBuilder.Entity<IncidentPingStats>().HasNoKey();
            modelBuilder.Entity<AdminTransactionType>().HasNoKey();
            modelBuilder.Entity<CompanyDetails>().HasNoKey();
            modelBuilder.Entity<RegisteredUser>().HasNoKey();
            modelBuilder.Entity<CompaniesStats>().HasNoKey();
            modelBuilder.Entity<CompanyMessageTransactionStats>().HasNoKey();
            //modelBuilder.Entity<CompanyTranscationType>().HasNoKey();
            modelBuilder.Entity<CompanyObject>().HasNoKey();
            modelBuilder.Entity<GroupUsers>().HasNoKey();
            modelBuilder.Entity<CompanyScimProfile>().HasNoKey();

            //modelBuilder.Entity<Location>().HasNoKey();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
