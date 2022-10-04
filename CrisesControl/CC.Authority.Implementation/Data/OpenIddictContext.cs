using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CC.Authority.Implementation.Data
{
    public class OpenIddictContext : DbContext, IDataProtectionKeyContext {
        public OpenIddictContext(DbContextOptions<OpenIddictContext> options)
            : base(options)
        {
            
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}