using Microsoft.EntityFrameworkCore;

namespace CC.Authority.Implementation.Data
{
    public class OpenIddictContext : DbContext
    {
        public OpenIddictContext(DbContextOptions<OpenIddictContext> options)
            : base(options)
        {
            
        }
    }
}