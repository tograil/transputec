using CrisesControl.Core.Billing;
using CrisesControl.Core.Communication;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Queues;
using CrisesControl.Core.Users;
using Microsoft.EntityFrameworkCore;
using CrisesControl.Core.Security;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Compatibility;

using CrisesControl.Core.ExTriggers;
using CrisesControl.Core.Jobs;
using CrisesControl.Core.Reports.Repositories;

namespace CrisesControl.Infrastructure.Context
{
    public class CrisesControlContext : DbContext
    {
        public CrisesControlContext()
        {
        }

        public CrisesControlContext(DbContextOptions<CrisesControlContext> options)
            : base(options)
        {
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

            modelBuilder.Entity<CompanyParameterItem>().HasNoKey();
            modelBuilder.Entity<UserConferenceItem>().HasNoKey();
            modelBuilder.Entity<IncidentPingStatsCount>().HasNoKey();
            modelBuilder.Entity<MessageAcknowledgements>().HasNoKey();
            modelBuilder.Entity<DataTablePaging>().HasNoKey().Ignore("data");
            modelBuilder.Entity<AcknowledgeReturn>().HasNoKey();

        }
    }
}
