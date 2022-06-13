﻿using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Communication;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Queues;
using CrisesControl.Core.Users;
using CrisesControl.Core.Communication;
using CrisesControl.Core.Incidents.SPResponse;
using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using CrisesControl.Core.Security;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Tasks;
using CrisesControl.Core.Tasks.SP_Response;
using CrisesControl.Core.Common;

using CrisesControl.Core.ExTriggers;
using CrisesControl.Core.Jobs;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Infrastructure.Context.Misc;
using CrisesControl.Core.Reports.SP_Response;
using CrisesControl.Core.Groups;

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

            //modelBuilder.Entity<Assets>().HasNoKey();
            modelBuilder.Entity<DeliverySummary>().HasNoKey();


            modelBuilder.Entity<LibIncident>().HasNoKey();



            //modelBuilder.Entity<Location>().HasNoKey();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
