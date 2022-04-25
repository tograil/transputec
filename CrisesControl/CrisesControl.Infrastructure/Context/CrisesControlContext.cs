using CrisesControl.Core.Billing;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Users;
using Microsoft.EntityFrameworkCore;


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
            modelBuilder.Entity<CompanyParameterItem>().HasNoKey();

        }
    }
}
