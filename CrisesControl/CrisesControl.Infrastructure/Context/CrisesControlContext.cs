﻿using CrisesControl.Core.Billing;
using CrisesControl.Core.Communication;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Users;
using CrisesControl.Core.Communication;
using CrisesControl.Core.Incidents.SP_Response;
using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using CrisesControl.Core.Security;
using CrisesControl.Core.CompanyParameters;

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
            modelBuilder.Entity<CompanySubscribedMethod>().HasNoKey();
            modelBuilder.Entity<UserMessageList>().HasNoKey().Ignore("SentBy");
            modelBuilder.Entity<UserFullName>().HasNoKey();
            modelBuilder.Entity<SOSItem>().HasNoKey();
            modelBuilder.Entity<ConferenceDetails>().HasNoKey();
            modelBuilder.Entity<CompanySecurityGroup>().HasNoKey();
            modelBuilder.Entity<CommsMethodPriority>().HasNoKey();
            modelBuilder.Entity<CascadingPlanReturn>().HasNoKey();
            modelBuilder.Entity<CompanyParameterItem>().HasNoKey();
            modelBuilder.Entity<UserConferenceItem>().HasNoKey();
            modelBuilder.Entity<IncidentPingStatsCount>().HasNoKey();


            modelBuilder.Entity<IncidentList>().HasNoKey();
            modelBuilder.Entity<AffectedLocation>().HasNoKey();//
            modelBuilder.Entity<UpdateIncidentStatusReturn>().HasNoKey();
            modelBuilder.Entity<IncidentDetails>().HasNoKey();
            modelBuilder.Entity<CommsMethods>().HasNoKey();

            modelBuilder.Entity<AckOption>().HasNoKey();
            modelBuilder.Entity<IncKeyCons>().HasNoKey();
            modelBuilder.Entity<IncidentAssetResponse>().HasNoKey();
            modelBuilder.Entity<ActionLsts>().HasNoKey();
            modelBuilder.Entity<IncidentParticipants>().HasNoKey();
            modelBuilder.Entity<UserFullName>().HasNoKey();






            //modelBuilder.Entity<Location>().HasNoKey();
        }
    }
}
