using System.Collections.Generic;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.SP_Response;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Object = CrisesControl.Core.Models.Object;

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
